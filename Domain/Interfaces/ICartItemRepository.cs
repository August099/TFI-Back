using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetByCartIdAsync(int cartId);
        Task<CartItem?> GetByIdAsync(int id);
        Task<CartItem?> UpdateAsync(CartItem cartItem);
        Task<CartItem?> AddAsync(CartItem cartItem);
        Task SaveChangesAsync();
    }
}