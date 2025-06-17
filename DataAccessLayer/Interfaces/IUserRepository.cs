using DataAccessLayer.Dtos;
using DataAccessLayer.Models;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(int? roleId);
        Task DeleteUserAsync(int professorId);
        Task<bool> EmailExistsAsync(string email);
        Task<CreatedUserDto> AddUserAsync(User user);


    }
}