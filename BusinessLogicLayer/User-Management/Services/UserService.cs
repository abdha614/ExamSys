using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Repositories;
using System.Net;
using DataAccessLayer.Dtos;

namespace BusinessLogicLayer.Services
{
    public class UserService : IUserService 
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHistoryService _historyService; // Add IHistoryService to log user actions
        private readonly IRoleService _roleService;


        public UserService(IUserRepository userRepository, IMapper mapper, IHistoryService historyService, IPasswordHasher<User> passwordHasher, IRoleService roleService)
        {
            _userRepository = userRepository;
            _mapper = mapper;   
            _passwordHasher = passwordHasher;
            _historyService = historyService;
            _roleService = roleService;
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
        public async Task<RegisteredUserDto> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            if (await _userRepository.EmailExistsAsync(registrationDto.Email))
            {
                throw new InvalidOperationException("An account with this email already exists.");
            }

            var user = _mapper.Map<User>(registrationDto);
            user.PasswordHash = _passwordHasher.HashPassword(user, registrationDto.Password);
            user.MustChangePassword = true;

            var createdUser = await _userRepository.AddUserAsync(user);

            await _historyService.LogActionAsync(new HistoryDto
            {
                UserId = createdUser.Id,
                Action = "User Registered",
                IpAddress = "172.1.1.1",
                Timestamp = DateTime.UtcNow
            });

            return new RegisteredUserDto
            {
                Id = createdUser.Id,
                Email = registrationDto.Email,
                RoleId = createdUser.RoleId
            };
        }



        public async Task<IEnumerable<UserDto>> GetUsersFilteredAsync(int? roleId)
        {
            // Fetch the filtered list of users from the repository
            var users = await _userRepository.GetUsersByRoleAsync(roleId);

            // Map the result to a DTO (Data Transfer Object) format
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        public async Task<bool> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(updateUserDto.Id);
            if (user == null)
            {
                return false; // User not found
            }

            //  Update user properties
            user.Email = updateUserDto.Email;
            user.RoleId = updateUserDto.RoleId;
            user.MustChangePassword = updateUserDto.MustChangePassword;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                //  Hash the new password if provided
                user.PasswordHash = _passwordHasher.HashPassword(user, updateUserDto.Password);
            }

            await _userRepository.UpdateAsync(user);
            return true;
        }
        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            //  Hash the new password inside the service layer
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.MustChangePassword = false; // Disable forced password change

            await _userRepository.UpdateAsync(user);
            return true;
        }
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            // Verify the current password using the password hasher
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return false; // Current password is incorrect
            }

            // Hash the new password
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

            // Update user record
            await _userRepository.UpdateAsync(user);

            return true; // Password change successful
        }
        public async Task<IEnumerable<UserDto>> GetProfessorsAsync()
        {
            var professors = await _userRepository.GetUsersByRoleAsync(1);
            return _mapper.Map<IEnumerable<UserDto>>(professors);
        }
    }
}
