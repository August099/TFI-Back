using Application.Models.Shift;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IShiftService
    {
        Task<List<ShiftDTO>> GetAllAsync();
        Task<ShiftDTO?> GetByIdAsync(int id);
        Task<List<ShiftDTO>> GetByClientIdAsync(int clientId);
        Task<List<ShiftDTO>> GetByVeterinaryIdAsync(int veterinaryId);
        Task<List<ShiftDTO>> GetByPetIdAsync(int petId);
        Task<List<ShiftDTO>> GetByStatusAsync(ShiftStatus status);
        Task<ShiftDTO?> AddAsync(CreateShiftDTO shift);
        Task<ShiftDTO?> UpdateAsync(ShiftDTO shift);
        Task<ShiftDTO?> DeleteAsync(int id);
    }
}