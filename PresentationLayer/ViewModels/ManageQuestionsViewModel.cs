using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PresentationLayer.ViewModels
{
    public class ManageQuestionsViewModel
    {
        // List of Questions
        public IEnumerable<QuestionViewModel> Questions { get; set; }

        // Dropdown filter options
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>(); // Initialize to avoid null referce
        public List<SelectListItem> QuestionTypes { get; set; }
        public List<SelectListItem> DifficultyLevels { get; set; }
        public List<SelectListItem> Lectures { get; set; } = new List<SelectListItem>(); // Lectures dropdown


        // Selected filter values

        public int? SelectedCourseId { get; set; }
        public int? SelectedDifficultyLevelId { get; set; }

        public int? SelectedQuestionTypeId { get; set; }

        public int? SelectedLectureId { get; set; } // Selected Lecture ID

    }

}
