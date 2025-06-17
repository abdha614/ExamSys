using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.ExamDto
{
    public class QuestionAutoFilterDto
    {
        public int TotalQuestions { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();

        // Optional: You can add statistics if needed
        public int EasyCount { get; set; }
        public int MediumCount { get; set; }
        public int HardCount { get; set; }
        public int McqCount { get; set; }
        public int TfCount { get; set; }
    }

}
