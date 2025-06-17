using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels.ExamViewModel
{
    public class CreateManualExamViewModel
    {
        //Exam Information
       [Required]
        public string? ExamTitle { get; set; }

        [Required]
        [Range(1, 300)]
        public int? DurationMinutes { get; set; }
        public string StudentName { get; set; }
        public string StudentId { get; set; }

        [Required]
        public DateTime? StartTime { get; set; }
       // Exam Details
        //public string? ExamTitle { get; set; }
        //public int? DurationMinutes { get; set; }
        //public DateTime? StartTime { get; set; }

        // Filters (multi-selection)
        public List<int> SelectedLectureIds { get; set; } = new List<int>();
        public List<int> SelectedDifficultyLevelIds { get; set; } = new List<int>();
        public List<int> SelectedQuestionTypeIds { get; set; } = new List<int>();

        // Optional filters
        [Required(ErrorMessage = "The Selected Course Name field is required.")]
        public int? SelectedCourseId { get; set; }
        [Required(ErrorMessage = "The Selected Category Name field is required.")]
        public int? SelectedCategoryId { get; set; }
        public string? courseName { get; set; }

        // Dropdown Data
        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Lectures { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> DifficultyLevels { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> QuestionTypes { get; set; } = new List<SelectListItem>();

        // Resulting Questions from Filter
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();

        // Selected Questions for Exam Creation
        public List<SelectedQuestionDto> SelectedQuestions { get; set; } = new List<SelectedQuestionDto>();


        public string TeacherName { get; set; }
        public string Semester { get; set; }
        public string CourseNamee { get; set; } // or SubjectName
    }
}
