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
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Category { get; set; }
        public string Course { get; set; }
        public string DifficultyLevel { get; set; }
        public string QuestionType { get; set; }
    }
}
