using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{
    public class ConfirmCodeViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the confirmation code.")]
        [MaxLength(6, ErrorMessage = "Code must be 6 digits.")]
        [MinLength(6, ErrorMessage = "Code must be 6 digits.")]
        public string Code { get; set; }
    }
}
