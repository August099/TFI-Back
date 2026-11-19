using System;
using System.Collections.Generic;
using System.Text;
using Application.Models.Message;

namespace Application.Interfaces
{
    public interface IChatMessageService
    {
        Task<List<MessageDTO>> GetByChatIdAsync(int chatId);
        Task<MessageDTO> GetByIdAsync(int id);
        Task<MessageDTO> AddAsync(CreateMessageDTO message, int senderId);
        Task<MessageDTO> UpdateAsync(int id, UpdateMessageDTO message);
    }
}