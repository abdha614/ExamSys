using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IConfirmationCodeRepository : IGenericRepository<EmailConfirmationCode>
    {
        Task<EmailConfirmationCode?> GetValidCodeAsync(string email, string code);
    }
}
