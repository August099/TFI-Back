using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;
using Application.Models.Message;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;

        public ChatMessageService(
            IMessageRepository messageRepository,
            IChatRepository chatRepository)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
        }

        public async Task<List<MessageDTO>> GetByChatIdAsync(int chatId)
        {
            if (chatId <= 0)
                throw new BadRequestException("El Id del chat no es válido.");

            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat is null)
                throw new NotFoundException($"No se encontró el chat con Id: {chatId}.");

            var messages = await _messageRepository.GetMessagesByChatIdAsync(chatId);
            return messages.Select(MapToDTO).ToList();
        }

        public async Task<MessageDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("El Id del mensaje no es válido.");

            var message = await _messageRepository.GetByIdAsync(id);
            if (message is null)
                throw new NotFoundException($"No se encontró el mensaje con Id: {id}.");

            return MapToDTO(message);
        }

        public async Task<MessageDTO> AddAsync(CreateMessageDTO message, int senderId)
        {
            if (message is null)
                throw new BadRequestException("Los datos del mensaje son inválidos.");

            if (senderId <= 0)
                throw new BadRequestException("El remitente no es válido.");

            if (message.ChatId <= 0)
                throw new BadRequestException("El Id del chat no es válido.");

            if (string.IsNullOrWhiteSpace(message.Text))
                throw new BadRequestException("El texto del mensaje no puede estar vacío.");

            var chat = await _chatRepository.GetByIdAsync(message.ChatId);
            if (chat is null)
                throw new NotFoundException($"No se encontró el chat con Id: {message.ChatId}.");
            var newMessage = new Message
            {
                ChatId = message.ChatId,
                Text = message.Text.Trim(),
                SendHour = DateTime.Now,
                SenderId = senderId
            };

            var added = await _messageRepository.AddAsync(newMessage);
            if (added is null)
                throw new BadRequestException("No se pudo crear el mensaje.");

            await _messageRepository.SaveChangesAsync();

            return MapToDTO(added);
        }

        public async Task<MessageDTO> UpdateAsync(int id, UpdateMessageDTO message)
        {
            if (id <= 0)
                throw new BadRequestException("El Id del mensaje no es válido.");

            if (message is null || string.IsNullOrWhiteSpace(message.Text))
                throw new BadRequestException("El texto del mensaje no puede estar vacío.");

            var existing = await _messageRepository.GetByIdAsync(id);
            if (existing is null)
                throw new NotFoundException($"No se encontró el mensaje con Id: {id}.");

            existing.Text = message.Text.Trim();

            var updated = await _messageRepository.UpdateAsync(existing);
            if (updated is null)
                throw new BadRequestException("No se pudo actualizar el mensaje.");

            await _messageRepository.SaveChangesAsync();

            return MapToDTO(updated);
        }

        private static MessageDTO MapToDTO(Message m)
        {
            return new MessageDTO
            {
                Id = m.Id,
                ChatId = m.ChatId,
                Text = m.Text,
                SendHour = m.SendHour,
                SenderId = m.SenderId
            };
        }
    }
}
