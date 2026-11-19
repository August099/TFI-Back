using Application.Models.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDTO?> GetByIdAsync(int id);
        Task<CartDTO?> GetOrCreateByClientIdAsync(int clientId);
    }
}