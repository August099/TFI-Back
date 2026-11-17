using Application.Interfaces;
using Application.Models.Chat;
using Application.Models.Message;
using Domain.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<List<ChatDTO>> GetAllAsync()
        {
            var chats = await _chatRepository.GetAllAsync();
            return chats.Select(MapToDTO).ToList();
        }

        public async Task<ChatDTO?> GetByIdAsync(int id)
        {
            var chat = await _chatRepository.GetByIdAsync(id);
            if (chat == null) return null;

            return MapToDTO(chat);
        }

        public async Task<List<ChatDTO>> GetByClientIdAsync(int clientId)
        {
            var chats = await _chatRepository.GetByClientIdAsync(clientId);
            return chats.Select(MapToDTO).ToList();
        }

        public async Task<List<ChatDTO>> GetByRecepcionistIdAsync(int receptionistId)
        {
            var chats = await _chatRepository.GetByRecepcionistIdAsync(receptionistId);
            return chats.Select(MapToDTO).ToList();
        }

        public async Task<ChatDTO?> AddAsync(CreateChatDTO chat)
        {
            if (chat == null) return null;

            var newChat = new Chat
            {
                ClientId = chat.ClientId,
                ReceptionistId = chat.ReceptionistId,
                Status = ChatStatus.Activo,
                CreationDate = DateTime.Now,
                Messages = new List<Message>()
            };

            var addedChat = await _chatRepository.AddAsync(newChat);
            await _chatRepository.SaveChangesAsync();
            if (addedChat == null || addedChat.Id == 0) return null;

            return MapToDTO(addedChat);
        }

        public async Task<ChatDTO?> UpdateAsync(ChatDTO chat)
        {
            var updateChat = await _chatRepository.GetByIdAsync(chat.Id);
            if (updateChat == null) return null;

            updateChat.ClientId = chat.ClientId;
            updateChat.ReceptionistId = chat.ReceptionistId;
            updateChat.Status = chat.Status;
            updateChat.Messages = chat.Messages.Select(m => new Message
            {
                SendHour = m.SendHour,
                Text = m.Text,
                SenderId = m.SenderId
            }).ToList();

            await _chatRepository.UpdateAsync(updateChat);
            await _chatRepository.SaveChangesAsync();

            return MapToDTO(updateChat);
        }

        public async Task<ChatDTO?> DeleteAsync(int id)
        {
            var chat = await _chatRepository.GetByIdAsync(id);
            if (chat == null) return null;

            await _chatRepository.DeleteAsync(id);
            await _chatRepository.SaveChangesAsync();

            return MapToDTO(chat);
        }

        // mapear chat a chatDTO
        private static ChatDTO MapToDTO(Chat c)
        {
            return new ChatDTO
            {
                Id = c.Id,
                ClientId = c.ClientId,
                ReceptionistId = c.ReceptionistId,
                Status = c.Status,
                CreationDate = c.CreationDate,
                Messages = c.Messages?.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SendHour = m.SendHour,
                    Text = m.Text,
                    SenderId = m.SenderId
                }).ToList() ?? new List<MessageDTO>()
            };
        }
    }
}