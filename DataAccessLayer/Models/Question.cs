using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int CourseId { get; set; }
        public int DifficultyLevelId { get; set; }
        public int QuestionTypeId { get; set; }
        public int CategoryId { get; set; } // Foreign key for Category
        public int LectureId { get; set; }

        // Foreign key for Professor 
        public int ProfessorId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Course Course { get; set; }
        public User Professor { get; set; } // Navigation property for User
        public DifficultyLevel DifficultyLevel { get; set; }
        public QuestionType QuestionType { get; set; }
        public Category Category { get; set; } // Navigation property
        public Lecture Lecture { get; set; } // Navigation property
        public ICollection<Answer> Answers { get; set; }
    }
}
