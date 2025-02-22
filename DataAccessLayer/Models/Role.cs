using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, ErrorMessage = "Role name can't be longer than 50 characters")]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>(); // Navigation property
    }
}