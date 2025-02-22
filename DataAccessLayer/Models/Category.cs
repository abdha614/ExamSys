using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    public class Category
    {
        public int Id { get; set; }

        [MaxLength(100)] // Limit the length of Name to 100 characters
        public string Name { get; set; }

        // Foreign key for Professor
        public int ProfessorId { get; set; }

        // Navigation property for the relationship
        public User Professor { get; set; } // Navigation property for User

        // Navigation property for the one-to-many relationship
        public ICollection<Course> Courses { get; set; }
        public ICollection<Question> Questions { get; set; } // Navigation property
    }
}
