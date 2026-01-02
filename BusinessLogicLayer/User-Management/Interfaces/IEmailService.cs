using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.User_Management.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        string ComposeProfessorWelcomeEmail(string email, string tempPassword, string loginUrl);

        string ComposeConfirmationCodeEmail(string recipientName, string code);
        Task SendConfirmationEmailAsync(string toEmail, string code);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);

    }

}
