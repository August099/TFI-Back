using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByIdAsync(int id)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id);
            if (cart == null)
                return null;
            return cart;
        }

        public async Task<Cart?> GetByClientIdAsync(int clientId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (cart == null)
                return null;
            return cart;
        }

        public async Task<Cart?> Add(Cart cart)
        {
            if (cart == null)
                return null;

            _context.Carts.Add(cart);
            return cart;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}