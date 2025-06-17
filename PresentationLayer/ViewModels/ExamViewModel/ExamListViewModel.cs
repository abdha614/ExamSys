using BusinessLogicLayer.Dtos.ExamDto;

namespace PresentationLayer.ViewModels.ExamViewModel
{
    public class ExamListViewModel
    {
        public List<ExamListDto> Exams { get; set; } = new List<ExamListDto>();
    }
}
