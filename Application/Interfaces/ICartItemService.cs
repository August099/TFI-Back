using Application.Models.CartItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICartItemService
    {
        Task<List<CartItemDTO>> GetByCartIdAsync(int cartId);
        Task<CartItemDTO?> GetByIdAsync(int id);
        Task<CartItemDTO?> UpdateAsync(CartItemDTO cartItem);
        Task<CartItemDTO?> AddAsync(int cartId, CreateCartItemDTO cartItem);
    }
}