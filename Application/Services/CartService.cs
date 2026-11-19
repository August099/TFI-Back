using Application.Interfaces;
using Application.Models.Cart;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartDTO?> GetByIdAsync(int id)
        {
            var cart = await _cartRepository.GetByIdAsync(id);
            if (cart == null)
                return null;
            return new CartDTO
            {
                Id = cart.Id,
                ClientId = cart.ClientId
            };
        }

        public async Task<CartDTO?> GetOrCreateByClientIdAsync(int clientId)
        {
            var cart = await _cartRepository.GetByClientIdAsync(clientId);
            if (cart == null)
            {
                cart = new Cart
                {
                    ClientId = clientId
                };

                await _cartRepository.Add(cart);
                await _cartRepository.SaveChangesAsync();
            }

            return new CartDTO
            {
                Id = cart.Id,
                ClientId = cart.ClientId
            };
        }
    }
}