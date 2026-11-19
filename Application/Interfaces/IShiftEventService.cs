using Domain.Entities;
using Application.Models.ShiftEvent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IShiftEventService
    {
        Task<ShiftEventDTO?> GetByIdAsync(int id);
        Task<List<ShiftEventDTO>> GetByShiftIdAsync(int shiftId);
        Task<List<ShiftEventDTO>> GetByUserIdAsync(int userId);
        Task<ShiftEventDTO?> AddAsync(CreateShiftEventDTO shiftEvent);
    }
}