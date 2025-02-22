using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Repositories;
using System.Net;

namespace BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHistoryService _historyService; // Add IHistoryService to log user actions


        public UserService(IUserRepository userRepository, IMapper mapper, IHistoryService historyService, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;   
            _passwordHasher = passwordHasher;
            _historyService = historyService;
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

      
        public async Task DeleteUserAsync(int userId)
        {
            await _userRepository.DeleteAsync(userId);
        }
        public async Task<UserDto> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registrationDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("An account with this email already exists.");
            }

            var user = _mapper.Map<User>(registrationDto);
            user.PasswordHash = _passwordHasher.HashPassword(user, registrationDto.Password);

            await _userRepository.AddAsync(user);

            // Log the registration action
            await _historyService.LogActionAsync(new HistoryDto
            {
                UserId = user.Id,
                Action = "User Registered",
                IpAddress = "172.1.1.1", // IP Address from the controller
                Timestamp = DateTime.UtcNow // Current timestamp
            });
            return _mapper.Map<UserDto>(user);

        }
        public async Task<IEnumerable<UserDto>> GetUsersFilteredAsync(int? roleId)
        {
            // Fetch the filtered list of users from the repository
            var users = await _userRepository.GetUsersByRoleAsync(roleId);

            // Map the result to a DTO (Data Transfer Object) format
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

    }
}
