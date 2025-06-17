using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dtos
{
    public class ExamDetailDto
    {
        public int Id { get; set; }
 
        public string Metadata { get; set; } // ✅ Added Metadata

        public List<ExamQuestionDto> Questions { get; set; } = new List<ExamQuestionDto>(); // Contains exam questions
    }

}
