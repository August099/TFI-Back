using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByIdAsync(int id);
        Task<Cart?> GetByClientIdAsync(int clientId);
        Task<Cart?> Add(Cart cart);
        Task SaveChangesAsync();
    }
}