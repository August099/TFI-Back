using Application.Models.OrderItem;
using Domain.Entities;

namespace Application.Models.Order
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? DeliveryDate { get; set; } = null;
        public DateTime? PreparationDate { get; set; } = null;
        public PaymentMethods PaymentMethod { get; set; } = PaymentMethods.Efectivo;
        public OrderStatus Status { get; set; } = OrderStatus.PendientePago;
        public decimal Total { get; set; }

        // 1:n
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }
}