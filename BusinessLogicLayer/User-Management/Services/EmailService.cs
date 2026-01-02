using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BusinessLogicLayer.User_Management.Interfaces;

namespace BusinessLogicLayer.User_Management.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = _configuration["EmailSettings:SmtpPass"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                var mailMessage = new MailMessage(fromEmail, toEmail, subject, body)
                {
                    IsBodyHtml = true // Set to true if you want rich HTML formatting
                };

                await client.SendMailAsync(mailMessage);
            }
        }
       
        public string ComposeProfessorWelcomeEmail(string email, string tempPassword, string loginUrl)
        {
            return $@"
    <html>
    <body style='font-family: Arial; color: #333;'>
        <h2 style='color: #2c3e50;'>Welcome to QBanker!</h2>
        <p>Hello <strong>Professor</strong>,</p>
        <p>You've been registered into the system with a temporary password.</p>

        <p><strong>🔐 Temporary Password:</strong> <code>{tempPassword}</code></p>
        <p><strong>🔗 Login URL:</strong> <a href='{loginUrl}' style='color: #2980b9;'>Click here to log in</a></p>

        <p style='color: darkred;'>⚠️ Please note: You will be required to change your password upon first login.</p>

     
        <p>Best regards,<br/>QBanker Admin Team</p>
    </body>
    </html>";
        }
        public async Task SendConfirmationEmailAsync(string toEmail, string code)
        {
            var subject = "Confirm Your Email - QBanker";
            var body = ComposeConfirmationCodeEmail("Professor", code); // You can make name dynamic later
            await SendEmailAsync(toEmail, subject, body);
        }

        public string ComposeConfirmationCodeEmail(string recipientName, string code)
        {
            return $@"
    <html>
    <body style='font-family: Arial; color: #333;'>
        <h2 style='color: #2c3e50;'>Email Confirmation</h2>
        <p>Hello <strong>{recipientName}</strong>,</p>
        <p>Here is your confirmation code to continue registration:</p>

        <p style='font-size: 20px; font-weight: bold; color: #007bff;'>{code}</p>

        <p>This code will expire in 15 minutes. If you didn't request it, please ignore this email.</p>

        <p>Best regards,<br/>QBanker Admin Team</p>
    </body>
    </html>";
        }
        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var subject = "Reset Your Password - QBanker";
            var body = ComposePasswordResetEmail("Professor", resetLink); // You can pass real name if available
            await SendEmailAsync(toEmail, subject, body);
        }
        private string ComposePasswordResetEmail(string recipientName, string resetLink)
        {
            return $@"
    <html>
    <body style='font-family: Arial; color: #333;'>
        <h2 style='color: #e74c3c;'>Password Reset Request</h2>
        <p>Hello <strong>{recipientName}</strong>,</p>
        <p>You requested a password reset. Click the button below to continue:</p>

        <p style='text-align: center;'>
            <a href='{resetLink}' style='display: inline-block; padding: 10px 20px; color: #fff; background-color: #007bff; text-decoration: none; border-radius: 5px;'>Reset Password</a>
        </p>

        <p>If you did not request this, you can ignore this email.</p>
        <p>Best regards,<br/>QBanker Admin Team</p>
    </body>
    </html>";
        }


    }

}



















//        public string ComposeProfessorWelcomeEmail(string email, string tempPassword, string loginUrl)
//        {
//            return $@"
//Hello Professor,

//You've been registered into the system with a temporary password.

//🔐 Temporary Password: {tempPassword}
//🔗 Login URL: {loginUrl}

//⚠️ Please note: You will be required to change your password upon first login.


//Best regards,  
//QBanker Admin Team
//";
//        }