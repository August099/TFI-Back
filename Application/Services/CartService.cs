using Application.Interfaces;
using Application.Models.Cart;
using Application.Models.CartItem;
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
        private readonly ICartItemRepository _cartItemRepository;
        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<CartDTO?> GetByIdAsync(int id)
        {
            var cart = await _cartRepository.GetByIdAsync(id);
            if (cart == null)
                return null;

            var items = await _cartItemRepository.GetByCartIdAsync(cart.Id);
            return MapDTO(cart, items);
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

            var items = await _cartItemRepository.GetByCartIdAsync(cart.Id);
            return MapDTO(cart, items);
        }

        private static CartDTO MapDTO(Cart cart, IEnumerable<CartItem> items) => new()
        {
            Id = cart.Id,
            ClientId = cart.ClientId,
            Items = items.Select(ci => new CartItemDTO
            {
                Id = ci.Id,
                CartId = ci.CartId,
                ItemId = ci.ItemId,
                Quantity = ci.Quantity
            }).ToList()
        };
    }
}