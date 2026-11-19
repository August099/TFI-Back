using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.CartItem
{
    public class CreateCartItemDTO
    {
        public int CartId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}