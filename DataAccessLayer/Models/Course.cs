using System.Collections.Generic;

namespace DataAccessLayer.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign key for Category
        public int CategoryId { get; set; }

        // Foreign key for Professor
        public int ProfessorId { get; set; }

        // Navigation properties
        public Category Category { get; set; }
        public User Professor { get; set; } // Navigation property for User
        public ICollection<Question> Questions { get; set; }
        public ICollection<Lecture> Lectures { get; set; }
        public ICollection<Exam> Exams { get; set; } // In Course and in Professor (User)

    }
}
