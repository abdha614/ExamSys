namespace PresentationLayer.ViewModels
{
    public class QuestionAiViewModel
    {
       
        public Guid Id { get; set; } = Guid.NewGuid(); // Required for deletion
       

        public string QuestionText { get; set; }
        public int LectureId { get; set; }
        public string LectureName { get; set; }

        public List<string>? Choices { get; set; }
        public string Answer { get; set; }

    }
}
