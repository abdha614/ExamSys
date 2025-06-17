using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.ExamDto
{
    public class ExamListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } // Exam name
        public string CourseName { get; set; } // Course name
        public int TotalQuestions { get; set; } // Number of questions
        public DateTime CreatedDate { get; set; }
    }

}
