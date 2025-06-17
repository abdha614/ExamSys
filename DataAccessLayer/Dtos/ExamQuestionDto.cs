using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dtos
{
    public class ExamQuestionDto
    {
        public int QuestionId { get; set; }  // Unique identifier for the question

        public int Order { get; set; }       // Display order in the exam
        public double Points { get; set; }   // Assigned points for the question
        public string QuestionType { get; set; } // ✅ Add this

        public string QuestionText { get; set; } // ✅ Stores question text
        public List<AnswerGettDto> Answers { get; set; } = new List<AnswerGettDto>(); // ✅ List of answers
    }
}
