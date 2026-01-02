using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.User_Management.Interfaces
{
    public interface IConfirmationCodeService
    {
        Task SaveCodeAsync(string email, string code);
        Task<bool> VerifyCodeAsync(string email, string code);
  

    }
}
