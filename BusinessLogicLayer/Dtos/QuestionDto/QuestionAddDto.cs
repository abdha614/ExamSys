using BusinessLogicLayer.Dtos.AnswerDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.QuestionDto
{  
    public class QuestionAddDto
    {
            public int Id { get; set; }
            public string Text { get; set; }
            public int QuestionTypeId { get; set; }
            public int DifficultyLevelId { get; set; }
            public int professorId { get; set; }
            public int CategoryId { get; set; }
            public int CourseId { get; set; }
        public List<AnswerAddDto> Answers { get; set; }
    }
    
}
