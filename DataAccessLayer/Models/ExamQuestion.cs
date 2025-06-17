using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class ExamQuestion
    {
        public int Id { get; set; }

        public int ExamId { get; set; }
        public int QuestionId { get; set; }

        public int Order { get; set; }
        public double Points { get; set; } = 1;

        public Exam Exam { get; set; }
        public Question Question { get; set; }
    }

}
