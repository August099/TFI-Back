using Application.Interfaces;
using Application.Models.ShiftEvent;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class ShiftEventService : IShiftEventService
    {
        private readonly IShiftEventRepository _shiftEventRepository;
        public ShiftEventService(IShiftEventRepository shiftEventRepository)
        {
            _shiftEventRepository = shiftEventRepository;
        }

        public async Task<ShiftEventDTO> GetByIdAsync(int shiftId)
        {
            var shiftsEvent = await _shiftEventRepository.GetByIdAsync(shiftId);

            return MapDTO(shiftsEvent);
        }

        public async Task<List<ShiftEventDTO>> GetByShiftIdAsync(int shiftId)
        {
            var shiftsEvent = await _shiftEventRepository.GetByShiftIdAsync(shiftId);

            return shiftsEvent.Select(MapDTO).ToList();

        }

        public async Task<List<ShiftEventDTO>> GetByUserIdAsync(int userId)
        {
            var shiftsEvent = await _shiftEventRepository.GetByUserIdAsync(userId);

            return shiftsEvent.Select(MapDTO).ToList();
        }

        public async Task<ShiftEventDTO?> AddAsync(CreateShiftEventDTO shiftEvent)
        {
            if (shiftEvent == null)
                return null;

            var newShiftEvent = new ShiftEvent
            {
                UserId = shiftEvent.UserId,
                ShiftId = shiftEvent.ShiftId,
                ReasonCancellationId = shiftEvent.ReasonCancellationId,
                ReasonConsultationId = shiftEvent.ReasonConsultationId,
                NewVeterinaryId = shiftEvent.NewVeterinaryId,
                OldVeterinaryId = shiftEvent.OldVeterinaryId,
                Type = shiftEvent.Type,
                Date = DateTime.UtcNow
            };

            var addedShiftEvent = await _shiftEventRepository.AddAsync(newShiftEvent);
            await _shiftEventRepository.SaveChangesAsync();
            if (addedShiftEvent == null)
                return null;

            return MapDTO(addedShiftEvent);
        }

        private static ShiftEventDTO MapDTO(ShiftEvent se)
        {
            return new ShiftEventDTO
            {
                Id = se.Id,
                UserId = se.UserId,
                ShiftId = se.ShiftId,
                ReasonCancellationId = se.ReasonCancellationId,
                ReasonConsultationId = se.ReasonConsultationId,
                NewVeterinaryId = se.NewVeterinaryId,
                OldVeterinaryId = se.OldVeterinaryId,
                Type = se.Type,
                Date = se.Date
            };
        }
    }
}