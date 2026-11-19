using Application.Interfaces;
using Application.Models;
using Application.Models.User;
using Domain.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapDTO).ToList();
        }

        public async Task<UserDTO?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return MapDTO(user);
        }

        public async Task<UserDTO?> GetByNameAsync(string name)
        {
            var user = await _userRepository.GetByNameAsync(name);
            if (user == null) return null;

            return MapDTO(user);
        }
        
        public async Task<UserDTO?> AddAsync(CreateUserDTO user)
        {
            if (user == null) return null;

            var newUser = new User
            {
                Dni= user.Dni,
                Name = user.Name,
                Email = user.Email,
                Telephone = user.Telephone,
                BirthDate = user.BirthDate
            };

            var addedUser = await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            if (addedUser == null) return null;

            return MapDTO(addedUser);
        }

        public async Task<UserDTO?> UpdateAsync(UserDTO user)
        {
            var updateUser = await _userRepository.GetByIdAsync(user.Id);
            if (updateUser == null) return null;

            updateUser.Rol = user.Rol;
            updateUser.Dni = user.Dni;
            updateUser.Name = user.Name;
            updateUser.Email = user.Email;
            updateUser.Telephone = user.Telephone;
            updateUser.State = user.State;
            updateUser.BirthDate = user.BirthDate;
            
            await _userRepository.UpdateAsync(updateUser);
            await _userRepository.SaveChangesAsync();

            return MapDTO(updateUser);
        }

        public async Task<UserDTO?> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            await _userRepository.DeleteAsync(id);
            await _userRepository.SaveChangesAsync();

            return MapDTO(user);
        }

        private static UserDTO MapDTO(User u)
        {
            return new UserDTO
            {
                Id = u.Id,
                Rol = u.Rol,
                Dni = u.Dni,
                Name = u.Name,
                Email = u.Email,
                Telephone = u.Telephone,
                State = u.State,
                BirthDate = u.BirthDate
            };
        }
    }
}