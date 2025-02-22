using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{
    public class AnswerAddViewModel
    {
        [Required(ErrorMessage = "Please enter a Answer text.")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Please select the correct answer.")]
        public bool IsCorrect { get; set; }
    }
}
