using BusinessLogicLayer.Dtos.AnswerDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.QuestionDto
{
    public class QuestionGetDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; } // Enriched with the corresponding name
        public int DifficultyLevelId { get; set; }
        public string DifficultyLevelName { get; set; } // Enriched with the corresponding name
        public int professorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // Enriched with the corresponding name
        public int CourseId { get; set; }
        public string CourseName { get; set; } // Enriched with the corresponding name
        public List<AnswerGetDto> Answers { get; set; }
    }


}


