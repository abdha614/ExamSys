namespace PresentationLayer.ViewModels
{
    public class MultipleChoiceQuestionViewModel
    {
        public string QuestionText { get; set; }
        public string DifficultyLevel { get; set; }
        public string CourseName { get; set; }
        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
    }

    

}
