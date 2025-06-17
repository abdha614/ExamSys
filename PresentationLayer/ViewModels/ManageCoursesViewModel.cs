using BusinessLogicLayer.Dtos;

namespace PresentationLayer.ViewModels
{
    public class ManageCoursesViewModel
    {
        public IEnumerable<CategoryWithCoursesDto> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }

    }
}
    