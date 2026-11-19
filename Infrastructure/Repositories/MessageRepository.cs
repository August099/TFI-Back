using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetAllAsync()
            => await _context.Messages.ToListAsync();

        public async Task<Message?> GetByIdAsync(int id)
            => await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int chatId)
            => await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SendHour)
                .ToListAsync();

        public async Task<Message?> AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            return message;
        }

        public Task<Message?> UpdateAsync(Message message)
        {
            if (message is null)
                return Task.FromResult<Message?>(null);

            _context.Messages.Update(message);
            return Task.FromResult<Message?>(message);
        }

        public async Task<Message?> DeleteAsync(int id)
        {
            var message = await GetByIdAsync(id);
            if (message is null)
                return null;

            _context.Messages.Remove(message);
            return message;
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
