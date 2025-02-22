using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class History
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign key to User
        public User User { get; set; } // Navigation property

        public string Action { get; set; }

        [Required(ErrorMessage = "Timestamp is required")]
        public DateTime Timestamp { get; set; } // Timestamp of the action

        // Optionally, if you want to log IP addresses or other data related to the event:
        public string IpAddress { get; set; }
    }
}
