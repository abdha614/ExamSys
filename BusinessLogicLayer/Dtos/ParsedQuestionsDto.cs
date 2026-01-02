using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class ParsedQuestionsDto
    {
        public List<QuestionGroupDto> QuestionGroups { get; set; } = new();
       // public int? CategoryId { get; set; }
        public int CourseId { get; set; }
       // public string LectureName { get; set; }
    }
}
