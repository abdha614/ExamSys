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
    public class ConfirmationCodeRepository : GenericRepository<EmailConfirmationCode>, IConfirmationCodeRepository
    {
        public ConfirmationCodeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<EmailConfirmationCode?> GetValidCodeAsync(string email, string code)
        {
            return await _dbSet.FirstOrDefaultAsync(c =>
                c.Email == email &&
                c.Code == code &&
                c.Expiration >= DateTime.UtcNow);
        }
    }
}
