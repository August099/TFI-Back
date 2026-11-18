using Application.Interfaces;
using Application.Models;
using Application.Models.Pet;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Application.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        public PetService(IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        public async Task<List<PetDTO>> GetAllAsync()
        {
            var pets = await _petRepository.GetAllAsync();
            return pets.Select(MapDTO).ToList();
        }

        public async Task<PetDTO?> GetByIdAsync(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet == null) return null;

            return MapDTO(pet);
        }

        public async Task<List<PetDTO>> GetByClientIdAsync(int clientId)
        {
            var pets = await _petRepository.GetByClientIdAsync(clientId);
            return pets.Select(MapDTO).ToList();
        }

        public async Task<PetDTO?> AddAsync(CreatePetDTO pet)
        {
            if (pet == null)
                return null;

            var newPet = new Pet
            {
                ClientId = pet.ClientId,
                Name = pet.Name,
                Race = pet.Race,
                Gender = pet.Gender,
                Species = pet.Species,
                BirthDate = pet.BirthDate,
            };

            var addedPet = await _petRepository.AddAsync(newPet);
            await _petRepository.SaveChangesAsync();
            if (addedPet == null || addedPet.Id == 0)
                return null;

            return MapDTO(addedPet);
        }

        public async Task<PetDTO?> UpdateAsync(PetDTO pet)
        {
            var updatePet = await _petRepository.GetByIdAsync(pet.Id);
            if (updatePet == null) return null;

            updatePet.ClientId = pet.ClientId;
            updatePet.Name = pet.Name;
            updatePet.Race = pet.Race;
            updatePet.Gender = pet.Gender;
            updatePet.Species = pet.Species;
            updatePet.BirthDate = pet.BirthDate;

            await _petRepository.UpdateAsync(updatePet);
            await _petRepository.SaveChangesAsync();

            return MapDTO(updatePet);
        }

        public async Task<PetDTO?> DeleteAsync(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if(pet == null) return null;

            await _petRepository.DeleteAsync(id);
            await _petRepository.SaveChangesAsync();

            return MapDTO(pet);
        }
 
        private static PetDTO MapDTO(Pet p)
        {
            return new PetDTO
            {
                Id = p.Id,
                ClientId = p.ClientId,
                Name = p.Name,
                Race = p.Race,
                Gender = p.Gender,
                Species = p.Species,
                BirthDate = p.BirthDate
            };
        }
    }
}