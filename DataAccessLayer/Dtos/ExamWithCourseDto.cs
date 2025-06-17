using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dtos
{
    public class ExamWithCourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CourseName { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CreatedDate { get; set; }

    }

}
