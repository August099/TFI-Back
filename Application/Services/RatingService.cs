using Application.Interfaces;
using Application.Models.Rating;
using Domain.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using Domain.Exceptions;

namespace Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<List<RatingDTO>> GetAllAsync()
        {
            var ratings = await _ratingRepository.GetAllAsync();
            return ratings.Select(MapDTO).ToList();
        }

        public async Task<RatingDTO?> GetByIdAsync(int id)
        {
            var rating = await _ratingRepository.GetByIdAsync(id);
            if (rating == null) return null;

            return MapDTO(rating);
        }

        public async Task<List<RatingDTO>> GetByClientIdAsync(int clientId)
        {
            var ratings = await _ratingRepository.GetByClientIdAsync(clientId);
            return ratings.Select(MapDTO).ToList();
        }
        public async Task<List<RatingDTO>> GetByShiftIdAsync(int shiftId)
        {
            var ratings = await _ratingRepository.GetByShiftIdAsync(shiftId);
            return ratings.Select(MapDTO).ToList();
        }

        public async Task<RatingDTO?> AddAsync(CreateRatingDTO rating)
        {
            if(rating.Score < 1 || rating.Score > 5)
            {
                throw new BadRequestException("El puntaje debe estar entre 1 y 5");
            }

            var newRating = new Rating
            {
                ClientId = rating.ClientId,
                ShiftId = rating.ShiftId,
                Date = DateTime.Now,
                Description = rating.Description,
                Score = rating.Score,
            };

            var addedRating = await _ratingRepository.AddAsync(newRating);
            await _ratingRepository.SaveChangesAsync();
            if (addedRating == null || addedRating.Id == 0)
                return null;

            return MapDTO(addedRating);
        }

        public async Task<RatingDTO?> UpdateAsync(RatingDTO rating)
        {
            if (rating.Score < 1 || rating.Score > 5)
            {
                throw new BadRequestException("El puntaje debe estar entre 1 y 5");
            }
            
            var updateRating = await _ratingRepository.GetByIdAsync(rating.Id);
            if (updateRating == null) return null;

            updateRating.Id = rating.Id;
            updateRating.ClientId = rating.ClientId;
            updateRating.ShiftId = rating.ShiftId;
            updateRating.Description = rating.Description;
            updateRating.Score = rating.Score;

            await _ratingRepository.UpdateAsync(updateRating);
            await _ratingRepository.SaveChangesAsync();

            return MapDTO(updateRating);
        }

        public async Task<RatingDTO?> DeleteAsync(int id)
        {
            var rating = await _ratingRepository.GetByIdAsync(id);
            if (rating == null) return null;

            await _ratingRepository.DeleteAsync(id);
            await _ratingRepository.SaveChangesAsync();

            return MapDTO(rating);
        }
    
        private static RatingDTO MapDTO(Rating r)
        {
            return new RatingDTO
            {
                Id = r.Id,
                ClientId = r.ClientId,
                ShiftId = r.ShiftId,
                Date = r.Date,
                Description = r.Description,
                Score = r.Score
            };
        }
    }
}