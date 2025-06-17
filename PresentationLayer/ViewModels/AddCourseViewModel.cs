using Microsoft.AspNetCore.Mvc.Rendering;

namespace PresentationLayer.ViewModels
{
    public class AddCourseViewModel
    {
        public int SelectedCategoryId { get; set; } // Selected category ID
        public List<SelectListItem> Categories { get; set; } // List of categories for dropdown
        public string CourseName { get; set; } // New course name
        public int ProfessorId { get; set; } // Professor ID
    }

}
