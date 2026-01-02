using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class QuestionGroupDto
    {
        public string Type { get; set; }
        public List<QuestionAIDto> Questions { get; set; } = new();
    }
}
