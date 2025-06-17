using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool MustChangePassword { get; set; } = false; // Default is false
        public int RoleId { get; set; } // Foreign key

        [ForeignKey("RoleId")]
        public Role Roles { get; set; } // Navigation property

        public ICollection<History> Histories { get; set; } = new List<History>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();

        // Navigation properties for the relationships with Category and Course
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Lecture> Lectures { get; set; }
        public ICollection<Exam> Exams { get; set; } 

    }
}
