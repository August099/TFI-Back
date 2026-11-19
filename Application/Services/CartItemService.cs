using Application.Interfaces;
using Application.Models.CartItem;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        public CartItemService(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        public async Task<List<CartItemDTO>> GetByCartIdAsync(int cartId)
        {
            var cartItems = await _cartItemRepository.GetByCartIdAsync(cartId);
            return cartItems.Select(MapDTO).ToList();
        }

        public async Task<CartItemDTO?> GetByIdAsync(int id)
        {
            var cart = await _cartItemRepository.GetByIdAsync(id);
            if (cart == null)
                return null;
            return MapDTO(cart);
        }
        public async Task<CartItemDTO?> AddAsync(int cartId, CreateCartItemDTO cartItem)
        {
            if (cartId <= 0)
                throw new BadRequestException("El Id del carro no es válido");

            if (cartItem.ItemId <= 0)
                throw new BadRequestException("El Id del Item no es válido");

            if (cartItem.Quantity <= 0)
                throw new BadRequestException("La cantidad del item debe ser mayor a 0");

            var newCartItem = new CartItem
            {
                CartId = cartId,
                ItemId = cartItem.ItemId,
                Quantity = cartItem.Quantity
            };

            var addedCartItem = await _cartItemRepository.AddAsync(newCartItem);
            await _cartItemRepository.SaveChangesAsync();
            if (addedCartItem == null || addedCartItem.Id <= 0)
                return null;

            return MapDTO(addedCartItem);
        }

        public async Task<CartItemDTO?> UpdateAsync(CartItemDTO cartItem)
        {
            if (cartItem.Id <= 0)
                throw new BadRequestException("El Id del CartItem es inválido");

            if (cartItem.CartId <= 0 || cartItem.ItemId <= 0)
                throw new BadRequestException("El Id del carro y/o el Id del Item no son válidos");

            if (cartItem.Quantity <= 0)
                throw new BadRequestException("La cantidad del item debe ser mayor a 0");

            var updateCartItem = await _cartItemRepository.GetByIdAsync(cartItem.Id);
            if (updateCartItem == null || updateCartItem.Id <= 0)
                return null;

            if (updateCartItem.Id != cartItem.Id || updateCartItem.CartId != cartItem.CartId)
                throw new BadRequestException("El Id del CartItem y/o del Cart no coinciden");

            updateCartItem.ItemId = cartItem.ItemId;
            updateCartItem.Quantity = cartItem.Quantity;

            await _cartItemRepository.UpdateAsync(updateCartItem);
            await _cartItemRepository.SaveChangesAsync();

            return MapDTO(updateCartItem);
        }

        private static CartItemDTO MapDTO(CartItem ci)
        {
            return new CartItemDTO
            {
                Id = ci.Id,
                CartId = ci.CartId,
                ItemId = ci.ItemId,
                Quantity = ci.Quantity
            };
        }
    }
}