using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using DataAccessLayer.Models;
using System;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHistoryService _historyService;
        private readonly IResetTokenRepository _resetTokenRepository;

        public AuthService(
            IUserRepository userRepository,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            IHistoryService historyService,
            IResetTokenRepository resetTokenRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _historyService = historyService;
            _resetTokenRepository = resetTokenRepository;
        }

        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password) != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            await _historyService.LogActionAsync(new HistoryDto
            {
                UserId = user.Id,
                Action = "User Logged In",
                IpAddress = "172.1.1.1",
                Timestamp = DateTime.UtcNow
            });

            return _mapper.Map<UserDto>(user);
        }

        public async Task SavePasswordResetTokenAsync(string email, string token)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return;

            await _resetTokenRepository.AddAsync(new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(5)
            });
        }

        public async Task<bool> IsResetTokenValidAsync(string token)
        {
            var tokenRecord = await _resetTokenRepository.GetByTokenAsync(token);
            return tokenRecord != null && tokenRecord.Expiration > DateTime.UtcNow;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var tokenRecord = await _resetTokenRepository.GetByTokenAsync(token);
            if (tokenRecord == null || tokenRecord.Expiration <= DateTime.UtcNow)
                return false;

            var user = await _userRepository.GetByIdAsync(tokenRecord.UserId);
            if (user == null) return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.UpdateAsync(user);
            await _resetTokenRepository.DeleteAsync(tokenRecord.Id); // You could also add a DeleteByTokenAsync

            await _historyService.LogActionAsync(new HistoryDto
            {
                UserId = user.Id,
                Action = "Password Reset",
                IpAddress = "172.1.1.1",
                Timestamp = DateTime.UtcNow
            });

            return true;
        }
    }
}
