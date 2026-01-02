using BusinessLogicLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IAuthService
    {
       
        Task<UserDto> LoginAsync(LoginDto loginDto);
        Task SavePasswordResetTokenAsync(string email, string token);
        Task<bool> IsResetTokenValidAsync(string token);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}