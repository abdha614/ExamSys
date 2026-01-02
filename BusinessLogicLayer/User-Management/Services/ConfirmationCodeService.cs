using BusinessLogicLayer.User_Management.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Threading.Tasks;

namespace BusinessLogicLayer.User_Management.Services
{
    public class ConfirmationCodeService : IConfirmationCodeService
    {
        private readonly IConfirmationCodeRepository _confirmationCodeRepository;
        private readonly IUserRepository _userRepository;
        private readonly TimeSpan _codeLifetime = TimeSpan.FromMinutes(5);

        public ConfirmationCodeService(
            IConfirmationCodeRepository confirmationCodeRepository,
            IUserRepository userRepository)
        {
            _confirmationCodeRepository = confirmationCodeRepository;
            _userRepository = userRepository;
        }

        public async Task SaveCodeAsync(string email, string code)
        {
            var expiry = DateTime.UtcNow.Add(_codeLifetime);
            var entity = new EmailConfirmationCode
            {
                Email = email,
                Code = code,
                Expiration = expiry
            };
            await _confirmationCodeRepository.AddAsync(entity);
        }

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            var match = await _confirmationCodeRepository.GetValidCodeAsync(email, code);
            if (match != null)
            {
                await _confirmationCodeRepository.DeleteAsync(match.Id); // Optional: delete after successful check
                return true;
            }
            return false;
        }
    }
}
