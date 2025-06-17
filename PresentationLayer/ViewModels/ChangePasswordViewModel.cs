
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{ 
public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}
}