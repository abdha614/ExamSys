using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class QuestionAIDto
    {
        public string Number { get; set; }
        public int LectureId { get; set; }
        public string LectureName { get; set; }

        public string QuestionText { get; set; }
        public List<string>? Choices { get; set; } // Optional for MCQ
        public string Answer { get; set; }

        
    }
}
