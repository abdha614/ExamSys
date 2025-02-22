using DataAccessLayer.Models;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetRoleByNameAsync(string roleName);
    }
}