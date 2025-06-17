using BusinessLogicLayer.Dtos.ExamDto;


namespace PresentationLayer.ViewModels.ExamViewModel
{
    public class ExamDetailsViewModel
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Semester { get; set; }
        public string TeacherName { get; set; }

        public List<ExamQuestionDtto> Questions { get; set; } = new List<ExamQuestionDtto>();
    }
}
