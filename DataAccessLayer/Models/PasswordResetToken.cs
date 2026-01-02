using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }                // Primary key
        public int UserId { get; set; }            // FK to User table
        public string Token { get; set; }          // Unique reset token (GUID or secure string)
        public DateTime Expiration { get; set; }   // Token expiry timestamp

        // Optional metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property (if using EF Core)
        public User User { get; set; }
    }
}
