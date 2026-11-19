using Application.Interfaces;
using Application.Models.Shift;
using Application.Models.ShiftEvent;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;
        public ShiftService(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task<List<ShiftDTO>> GetAllAsync()
        {
            var shifts = await _shiftRepository.GetAllAsync();
            return shifts.Select(MapDTO).ToList();
        }

        public async Task<ShiftDTO?> GetByIdAsync(int id)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            if (shift == null) return null;

            return MapDTO(shift);
        }

        public async Task<List<ShiftDTO>> GetByClientIdAsync(int clientId)
        {
            var shifts = await _shiftRepository.GetByClientIdAsync(clientId);
            return shifts.Select(MapDTO).ToList();
        }

        public async Task<List<ShiftDTO>> GetByVeterinaryIdAsync(int veterinaryId)
        {
            var shifts = await _shiftRepository.GetByVeterinaryIdAsync(veterinaryId);
            return shifts.Select(MapDTO).ToList();
        }

        public async Task<List<ShiftDTO>> GetByPetIdAsync(int petId)
        {
            var shifts = await _shiftRepository.GetByPetIdAsync(petId);
            return shifts.Select(MapDTO).ToList();
        }

        public async Task<List<ShiftDTO>> GetByStatusAsync(ShiftStatus status)
        {
            var shifts = await _shiftRepository.GetByStatusAsync(status);
            return shifts.Select(MapDTO).ToList();
        }


        public async Task<ShiftDTO?> AddAsync(CreateShiftDTO shift) 
        {
            if (shift == null) return null;

            var newShift = new Shift
            {
                PetId = shift.PetId,
                VeterinaryId = shift.VeterinaryId,
                ClientId = shift.ClientId,
                ServiceId = shift.ServiceId,
                Status = ShiftStatus.Pendiente,
                ClientObs = shift.ClientObs,
                InitialDate = shift.InitialDate,
                EndDate = null
            };

            var addedShift = await _shiftRepository.AddAsync(newShift);
            await _shiftRepository.SaveChangesAsync();
            if (addedShift == null || addedShift.Id == 0) return null;

            return MapDTO(addedShift);
        }

        public async Task<ShiftDTO?> UpdateAsync(ShiftDTO shift)
        {
            if (shift == null) return null;

            var updateShift = await _shiftRepository.GetByIdAsync(shift.Id);
            if (updateShift == null) return null;

            updateShift.PetId = shift.PetId;
            updateShift.VeterinaryId = shift.VeterinaryId;
            updateShift.ClientId = shift.ClientId;
            updateShift.ServiceId = shift.ServiceId;
            updateShift.Status = shift.Status;
            updateShift.ClientObs = shift.ClientObs;
            updateShift.InitialDate = shift.InitialDate;
            updateShift.EndDate = shift.EndDate;

            await _shiftRepository.UpdateAsync(updateShift);
            await _shiftRepository.SaveChangesAsync();

            return MapDTO(updateShift);
        }

        public async Task<ShiftDTO?> DeleteAsync(int id)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            if (shift == null) return null;

            await _shiftRepository.DeleteAsync(shift.Id);
            await _shiftRepository.SaveChangesAsync();
            return MapDTO(shift);
        }

        private static ShiftDTO MapDTO(Shift s)
        {
            return new ShiftDTO
            {
                Id = s.Id,
                PetId = s.PetId,
                VeterinaryId = s.VeterinaryId,
                ClientId = s.ClientId,
                ServiceId = s.ServiceId,
                Status = s.Status,
                ClientObs = s.ClientObs,
                InitialDate = s.InitialDate,
                EndDate = s.EndDate
            };
        }
    }
}