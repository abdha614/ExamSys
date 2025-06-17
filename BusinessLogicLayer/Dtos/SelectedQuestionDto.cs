using BusinessLogicLayer.Dtos.AnswerDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class SelectedQuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }

        public double Points { get; set; }

        // Optional extras (for validation or audit/logging)
    //    public int? LectureId { get; set; }
        public string LectureName { get; set; }  // Adding LectureName

    //    public int? QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }  // Adding QuestionTypeName

   //     public int? DifficultyLevelId { get; set; }
        public string DifficultyLevelName { get; set; }  // Adding DifficultyLevelName
        public List<AnswerGetDto> Answers { get; set; }

        public int OrderIndex { get; set; } // NEW


    }
}
