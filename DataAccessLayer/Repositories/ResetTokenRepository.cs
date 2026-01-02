using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ResetTokenRepository : GenericRepository<PasswordResetToken>, IResetTokenRepository
    {
        public ResetTokenRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PasswordResetToken> GetByTokenAsync(string token)
        {
            return await _context.PasswordResetTokens
                .Include(t => t.User) // Optional if you want access to User from token
                .FirstOrDefaultAsync(t => t.Token == token);
        }
    }
}



