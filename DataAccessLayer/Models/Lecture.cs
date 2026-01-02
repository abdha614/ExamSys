using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Lecture
    {
        public int Id { get; set; }
        public string LectureName { get; set; } 
        // Foreign key for Course
        public int CourseId { get; set; }
        public int ProfessorId { get; set; }

        // Navigation property for User (Professor)
        public User Professor { get; set; }

        // Navigation property
        public Course Course { get; set; }

        public ICollection<Question> Questions { get; set; }
        public ICollection<LectureFile> Files { get; set; } = new List<LectureFile>();


    }
}
