namespace PresentationLayer.ViewModels
{
    public class AnswerViewModel
    {
        public int AnswerID { get; set; }
        public string AnswerText { get; set; }
        public int QuestionID { get; set; }
        public bool IsCorrect { get; set; }
    }
}
