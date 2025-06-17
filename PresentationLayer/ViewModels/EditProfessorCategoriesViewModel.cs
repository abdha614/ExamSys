using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{
    public class EditProfessorCategoriesViewModel
    {
        public int ProfessorId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [Display(Name = "Enter Category")]
        public string NewCategoryName { get; set; }

        public List<CategoryViewModel> Categories { get; set; }
    }
}
