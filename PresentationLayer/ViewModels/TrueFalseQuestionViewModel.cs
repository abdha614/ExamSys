namespace PresentationLayer.ViewModels
{
    public class TrueFalseQuestionViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string CourseName { get; set; }
        public string DifficultyLevel { get; set; }
        public string QuestionText { get; set; }
        public bool CorrectAnswer { get; set; }
    }
}
