using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.ExamDto
{
    public class ExamAddDto
    {
        public string ExamTitle { get; set; }
        public int professorId { get; set; }
        public int? SelectedCourseId { get; set; }

        public string CourseNamee { get; set; } // or SubjectName
        public string Semester { get; set; }
        public string TeacherName { get; set; }

        public List<SelectedQuestionDto> SelectedQuestions { get; set; } = new List<SelectedQuestionDto>();
    }
}
