using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.User_Management.Interfaces
{
    public interface ISignupNotificationService
    {
        Task SaveSignupRequestAsync(string email);
        Task<List<SignupNotificationDto>> GetUnhandledRequestsAsync();
        Task MarkAsHandledAsync(int id);
        Task MarkAsHandledByEmailAsync(string email);
        Task<List<SignupNotificationDto>> GetAllRequestsAsync();


    }
}
