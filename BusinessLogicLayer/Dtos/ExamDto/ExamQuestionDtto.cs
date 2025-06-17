using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.ExamDto
{
    public class ExamQuestionDtto
    {
        public int QuestionId { get; set; }      // Unique identifier for the question
        public int Order { get; set; }           // Display order in the exam
        public double Points { get; set; }       // Points assigned to the question
        public string QuestionType { get; set; } // ✅ Add this
        public string QuestionText { get; set; }

        public List<AnswerGettDtto> Answers { get; set; } = new List<AnswerGettDtto>(); // Answers for the question
    }

}
