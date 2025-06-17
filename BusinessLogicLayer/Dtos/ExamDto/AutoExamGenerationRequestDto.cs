using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.ExamDto
{
    public class AutoExamGenerationRequestDto
    {
        public int ProfessorId { get; set; }

        public int? SelectedCategoryId { get; set; }

        public int? SelectedCourseId { get; set; }

        public List<int> SelectedLectureIds { get; set; } = new List<int>();

        public int? TotalQuestions { get; set; }

      
        public int? McqCount { get; set; }
        public int? TfCount { get; set; }
        ///
        public int TotalMcq { get; set; }
        public int McqEasy { get; set; }
        public int McqMedium { get; set; }
        public int McqHard { get; set; }

        // TF Breakdown
        public int TotalTf { get; set; }
        public int TfEasy { get; set; }
        public int TfMedium { get; set; }
        public int TfHard { get; set; }
    }
}
