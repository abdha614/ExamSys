using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ISignupNotificationRepository : IGenericRepository<SignupNotification>
    {
        Task<bool> ExistsUnHandledEmailAsync(string email);
        Task<List<SignupNotification>> GetAllUnhandledAsync();
        Task<SignupNotification?> GetByIdAsync(int id);
        Task<SignupNotification?> GetUnhandledByEmailAsync(string email);
        Task<List<SignupNotification>> GetAlllAsync();


    }
}
