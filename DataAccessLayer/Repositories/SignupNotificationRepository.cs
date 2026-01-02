using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class SignupNotificationRepository : GenericRepository<SignupNotification>, ISignupNotificationRepository
    {
        public SignupNotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsUnHandledEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(n => n.Email == email && !n.IsHandled);
        }

        public async Task<List<SignupNotification>> GetAllUnhandledAsync()
        {
            return await _dbSet
                .Where(n => !n.IsHandled)
                .OrderByDescending(n => n.RequestedAt)
                .ToListAsync();
        }

        public async Task<SignupNotification?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(n => n.Id == id);
        }
        public async Task<SignupNotification?> GetUnhandledByEmailAsync(string email)
        {
            return await _context.SignupNotifications
                .FirstOrDefaultAsync(n => n.Email == email && !n.IsHandled);
        }
        public async Task<List<SignupNotification>> GetAlllAsync()
        {
            return await _dbSet
                .OrderByDescending(n => n.RequestedAt)
                .ToListAsync();
        }

    }
}
