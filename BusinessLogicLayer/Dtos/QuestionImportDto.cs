using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class QuestionImportDto
    {
        public string QuestionText { get; set; }
        public string Category { get; set; }
        public string Course { get; set; }
        public string DifficultyLevel { get; set; }
        public string QuestionType { get; set; }
        public List<string> Answers { get; set; } = new(); // Dynamic List of Answers
        public int CorrectAnswerIndex { get; set; }
    }
}