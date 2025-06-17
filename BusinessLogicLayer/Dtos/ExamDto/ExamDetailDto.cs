using BusinessLogicLayer.Dtos.QuestionDto;
using DataAccessLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.ExamDto
{
    public class ExamDetailDtto
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Semester { get; set; }
        public string TeacherName { get; set; }
        public List<ExamQuestionDtto> Questions { get; set; } = new List<ExamQuestionDtto>();
    }

}
