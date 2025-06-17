using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels.ExamViewModel
{
    public class CreateAutoExamViewModel
    {
        
        public string CourseNamee { get; set; }

        
        public string TeacherName { get; set; }

        
        public string Semester { get; set; }

        
        [Required]
        public string? ExamTitle { get; set; }

        
        [Required(ErrorMessage = "The Selected Category Name field is required.")]
        public int? SelectedCategoryId { get; set; }

        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Lectures { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> DifficultyLevels { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> QuestionTypes { get; set; } = new List<SelectListItem>();
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();


        
        [Required(ErrorMessage = "The Selected Course Name field is required.")]
        public int? SelectedCourseId { get; set; }
        public List<int>? SelectedLectureIds { get; set; }
        public int? TotalQuestions { get; set; }
        public decimal? PointsPerQuestion { get; set; }


        public string courseName { get; set; }

        // Difficulty Distribution
      
        ///////
     
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
