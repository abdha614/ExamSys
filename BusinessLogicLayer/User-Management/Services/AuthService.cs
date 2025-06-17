using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using DataAccessLayer.Models;

namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHistoryService _historyService; // Add IHistoryService to log user actions


        public AuthService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher, IHistoryService historyService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _historyService = historyService;
        }

        
        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password) != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Log the login action with actual time
            await _historyService.LogActionAsync(new HistoryDto
            {
                UserId = user.Id,
                Action = "User Logged In",
                IpAddress = "172.1.1.1", // Capture the user's IP address here
                Timestamp = DateTime.UtcNow // Capture the actual timestamp when action occurs
            });

            return _mapper.Map<UserDto>(user);
        }

    }
}