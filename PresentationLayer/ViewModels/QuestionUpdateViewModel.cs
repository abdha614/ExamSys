using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{
    public class QuestionUpdateViewModel
    {
        [Required]
        public int Id { get; set; } // Add this property

        [Required]
        public string Text { get; set; }

        [Required]
        public int QuestionTypeId { get; set; }

        [Required]
        public int DifficultyLevelId { get; set; }

        [Required]
        public int professorId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public List<AnswerAddViewModel> Answers { get; set; }

        [Required]
        public int CorrectAnswerIndex { get; set; }
    }
}
