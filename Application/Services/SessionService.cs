using Application.Interfaces;
using Application.Models.Session;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<List<SessionDTO>> GetAllAsync()
        {
            var sessions = await _sessionRepository.GetAllAsync();

            return sessions.Select(MapDTO).ToList();
        }

        public async Task<SessionDTO?> GetByIdAsync(int id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null) return null;

            return MapDTO(session);
        }

        public async Task<SessionDTO?> GetByTokenAsync(string token)
        {
            var session = await _sessionRepository.GetByTokenAsync(token);
            if (session == null) 
                return null;

            return MapDTO(session);
        }

        public async Task<List<SessionDTO>> GetActiveSessionsByUserIdAsync(int userId)
        {
            var sessions = await _sessionRepository.GetActiveSessionsByUserIdAsync(userId);
            return sessions.Select(MapDTO).ToList();
        }

        public async Task<SessionDTO?> AddAsync(CreateSessionDTO session)
        {
            if (session == null)
                return null;

            var newSession = new Session
            {
                UserId = session.UserId,
                Token = session.Token,
                EmissionDate = DateTime.Now,
                RevocationDate = null,
                ExpirationDate = session.ExpirationDate
            };

            var addedSession = await _sessionRepository.AddAsync(newSession);
            await _sessionRepository.SaveChangesAsync();
            if (addedSession == null) 
                return null;

            return MapDTO(addedSession);
        }

        public async Task<SessionDTO?> UpdateAsync(SessionDTO session)
        {
            var updateSession = await _sessionRepository.GetByIdAsync(session.Id);
            if (updateSession == null) return null;

            updateSession.UserId = session.UserId;
            updateSession.Token = session.Token;
            updateSession.EmissionDate = session.EmissionDate;
            updateSession.RevocationDate = session.RevocationDate;
            updateSession.ExpirationDate = session.ExpirationDate;

            await _sessionRepository.UpdateAsync(updateSession);
            await _sessionRepository.SaveChangesAsync();

            return MapDTO(updateSession);
        }

        public async Task<SessionDTO?> DeleteAsync(int id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if(session == null) return null;

            await _sessionRepository.DeleteAsync(id);
            await _sessionRepository.SaveChangesAsync();

            return MapDTO(session);
        }

        private static SessionDTO MapDTO(Session s)
        {
            return new SessionDTO
            {
                Id = s.Id,
                UserId = s.UserId,
                Token = s.Token,
                EmissionDate = s.EmissionDate,
                RevocationDate = s.RevocationDate,
                ExpirationDate = s.ExpirationDate
            };
        }
    }
}