using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{
    public class RequestConfirmationViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid institutional email.")]
        public string Email { get; set; }
    }
}
