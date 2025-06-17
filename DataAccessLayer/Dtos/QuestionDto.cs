using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dtos
{
    public class QuestionDto
    {
        public int Id { get; set; }                     // Question ID
        public string Text { get; set; }                // Question text/content
        public int QuestionTypeId { get; set; }        // Type of the question (e.g., MCQ, True/False)
        public string QuestionTypeName { get; set; }    // Name of the question type
        public int DifficultyLevelId { get; set; }     // Difficulty level ID
        public string DifficultyLevelName { get; set; } // Difficulty level name (e.g., Easy, Medium, Hard)
        public DateTime CreatedDate { get; set; }       // Date the question was created
        public int CategoryId { get; set; }            // Category ID
        public string CategoryName { get; set; }        // Name of the category
        public int CourseId { get; set; }              // Course ID
        public string CourseName { get; set; }          // Name of the course
        public int LectureId { get; set; }          // New field
        public string LectureName { get; set; }     // Optional field for lecture name
        public int professorId { get; set; }


        public List<AnswerGettDto> Answers { get; set; }

    }

}
