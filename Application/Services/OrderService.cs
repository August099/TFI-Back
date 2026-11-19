using Application.Interfaces;
using Application.Models.Order;
using Application.Models.OrderItem;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System.Data;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        public OrderService(IOrderRepository orderRepository, IItemRepository itemRepository, ICartRepository cartRepository, ICartItemRepository cartItemRepository)
        {
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<List<OrderDTO>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapDTO).ToList();
        }

        public async Task<OrderDTO?> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

            return MapDTO(order);
        }

        public async Task<List<OrderDTO>> GetByClientIdAsync(int clientId)
        {
            var orders = await _orderRepository.GetByClientIdAsync(clientId);
            return orders.Select(MapDTO).ToList();
        }
        public async Task<List<OrderDTO>> GetByStateAsync(OrderStatus status)
        {
            var orders = await _orderRepository.GetByStateAsync(status);
            return orders.Select(MapDTO).ToList();
        }

        public async Task<OrderDTO?> AddAsync(CreateOrderDTO order)
        {
            if (order == null) return null;

            if (!PaymentMethods.IsDefined(order.PaymentMethod))
            {
                throw new BadRequestException($"El metodo de pago: {order.PaymentMethod} no es válido");
            }

            if (!OrderStatus.IsDefined(order.Status))
            {
                throw new BadRequestException($"El estado de la orden: {order.Status} no es válido");
            }

            var cart = await _cartRepository.GetByClientIdAsync(order.ClientId)
        ?? throw new NotFoundException($"El cliente con id {order.ClientId} no tiene un carrito");

            var cartItems = (await _cartItemRepository.GetByCartIdAsync(cart.Id)).ToList();
            if (cartItems.Count == 0)
                throw new ConflictException("El carrito está vacío");

            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var cartItem in cartItems)
            {
                var itemDB = await _itemRepository.GetByIdAsync(cartItem.ItemId)
                    ?? throw new NotFoundException($"El Item con el id: {cartItem.ItemId} no existe");

                if (itemDB.Stock < cartItem.Quantity)
                    throw new ConflictException($"El stock del Item {itemDB.Name} no es suficiente, Stock {itemDB.Stock}");

                itemDB.Stock -= cartItem.Quantity;
                await _itemRepository.UpdateAsync(itemDB);

                total += itemDB.Price * cartItem.Quantity;

                orderItems.Add(new OrderItem
                {
                    ItemId = cartItem.ItemId,
                    Amount = cartItem.Quantity,
                    UnitPrice = itemDB.Price
                });
            }

            var newOrder = new Order
            {
                ClientId = order.ClientId,
                OrderDate = DateTime.Now,
                DeliveryDate = order.DeliveryDate,
                PreparationDate = order.PreparationDate,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                Total = total,
                Items = orderItems
            };

            await _orderRepository.AddAsync(newOrder);

            // vaciar el carrito
            await _cartItemRepository.DeleteRangeAsync(cartItems);

            await _orderRepository.SaveChangesAsync();

            return MapDTO(newOrder);
        }
 
        public async Task<OrderDTO?> UpdateAsync(OrderDTO order)
        {
            var updateOrder = await _orderRepository.GetByIdAsync(order.Id); 
            if(updateOrder == null) return null;
        
            updateOrder.DeliveryDate = order.DeliveryDate;
            updateOrder.PreparationDate = order.PreparationDate;
            updateOrder.PaymentMethod = order.PaymentMethod;
            updateOrder.Status = order.Status;

            await _orderRepository.UpdateAsync(updateOrder);
            await _orderRepository.SaveChangesAsync();

            return MapDTO(updateOrder);
        }

        public async Task<OrderDTO?> DeleteAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null) return null;

            await _orderRepository.DeleteAsync(id);
            await _orderRepository.SaveChangesAsync();

            return MapDTO(order);
        }
        private static OrderDTO MapDTO(Order o)
        {
            return new OrderDTO
            {
                Id = o.Id,
                ClientId = o.ClientId,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                PreparationDate = o.PreparationDate,
                PaymentMethod = o.PaymentMethod,
                Status = o.Status,
                Total = o.Total,
                Items = o.Items.Select(oi => new OrderItemDTO
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ItemId = oi.ItemId,
                    Amount = oi.Amount,
                    UnitPrice = oi.UnitPrice,
                }).ToList()
            };
        }
    }
}