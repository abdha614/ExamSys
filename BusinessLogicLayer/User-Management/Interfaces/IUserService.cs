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
        Task<bool> UpdateUserAsync(UpdateUserDto updateUserDto); 
        Task DeleteUserAsync(int userId);
        Task<RegisteredUserDto> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<IEnumerable<UserDto>> GetUsersFilteredAsync(int? roleId);
        Task<bool> ChangePasswordAsync(int userId, string newPassword);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<IEnumerable<UserDto>> GetProfessorsAsync(); 


    }
}