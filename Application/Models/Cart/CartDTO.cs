using Application.Models.CartItem;

namespace Application.Models.Cart
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public List<CartItemDTO> Items { get; set; } = [];
    }
}
