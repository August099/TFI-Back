using Application.Models.OrderItem;
using Domain.Entities;

namespace Application.Models.Order
{
    public class CreateOrderDTO
    {
        public int ClientId { get; set; }
        public DateTime? DeliveryDate { get; set; } = null;
        public DateTime? PreparationDate { get; set; } = null;
        public PaymentMethods PaymentMethod { get; set; } = PaymentMethods.Efectivo;
        public OrderStatus Status { get; set; } = OrderStatus.PendientePago;
        public List<CreateOrderItemDTO> Items { get; set;} = new List<CreateOrderItemDTO>();
    }
}