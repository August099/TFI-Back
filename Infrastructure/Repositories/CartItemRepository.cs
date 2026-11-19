using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;
        public CartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetByCartIdAsync(int cartId)
        {
            var cartItems = await _context.CartItems.Where(c => c.CartId == cartId).ToListAsync();
            return cartItems;
        }
        public async Task<CartItem?> GetByIdAsync(int id)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id);
            if (cartItem == null)
                return null;

            return cartItem;
        }
        public async Task<CartItem?> AddAsync(CartItem cartItem)
        {
            if (cartItem == null)
                return null;

            _context.CartItems.Add(cartItem);
            return cartItem;
        }
        public async Task<CartItem?> UpdateAsync(CartItem cartItem)
        {
            if (cartItem == null)
                return null;

            _context.CartItems.Update(cartItem);
            return cartItem;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}