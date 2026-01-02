using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class QuestionSectionDto
    {
        public string Category { get; set; }      // e.g. "MCQ (Easy)"
        public List<string> Questions { get; set; } = new();
    }

}
