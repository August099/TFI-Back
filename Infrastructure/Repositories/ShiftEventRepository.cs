using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ShiftEventRepository : IShiftEventRepository
    {
        private readonly ApplicationDbContext _context;
        public ShiftEventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShiftEvent?> GetByIdAsync(int id)
        {
            var shiftEvent = await _context.ShiftEvents.FirstOrDefaultAsync(se => se.Id == id);
            return shiftEvent;
        }

        public async Task<List<ShiftEvent>> GetByShiftIdAsync(int shiftId)
        {
            var shiftEvents = await _context.ShiftEvents.Where(se => se.ShiftId == shiftId).ToListAsync();
            return shiftEvents;
        }

        public async Task<List<ShiftEvent>> GetByUserIdAsync(int userId)
        {
            var shiftEvents = await _context.ShiftEvents.Where(se => se.UserId == userId).ToListAsync();
            return shiftEvents;
        }

        public async Task<ShiftEvent?> AddAsync(ShiftEvent shiftEvent)
        {
            if (shiftEvent == null) return null;

            await _context.ShiftEvents.AddAsync(shiftEvent);
            return shiftEvent;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}