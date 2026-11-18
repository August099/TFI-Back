using Application.Models.Order;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetAllAsync();
        Task<OrderDTO?> GetByIdAsync(int id);
        Task<List<OrderDTO>> GetByClientIdAsync(int clientId);
        Task<List<OrderDTO>> GetByStateAsync(OrderStatus status);
        Task<OrderDTO?> AddAsync(CreateOrderDTO order);
        Task<OrderDTO?> UpdateAsync(OrderDTO order);
        Task<OrderDTO?> DeleteAsync(int id);
    }
}