using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task DeleteUserAsync(int userId);
        Task<UserDto> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<IEnumerable<UserDto>> GetUsersFilteredAsync(int? roleId);
    }
}