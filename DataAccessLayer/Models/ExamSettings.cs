using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class ExamSettings
    {
        public int Id { get; set; }
        public int ExamId { get; set; }

        public int EasyQuestionCount { get; set; }
        public int MediumQuestionCount { get; set; }
        public int HardQuestionCount { get; set; }

        public string AllowedQuestionTypesJson { get; set; }
        public string IncludedCategoryIdsJson { get; set; }
        public string IncludedLectureIdsJson { get; set; }

        public Exam Exam { get; set; }
    }

}
