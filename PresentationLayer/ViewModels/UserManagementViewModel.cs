using Microsoft.AspNetCore.Mvc.Rendering;

namespace PresentationLayer.ViewModels
{
    public class UserManagementViewModel
    {
        public int? SelectedRoleId { get; set; } // Stores the selected role
        public IEnumerable<UserViewModel> Users { get; set; } // Stores the list of users
        public IEnumerable<SelectListItem> Roles { get; set; } // Stores the list of roles
    }
}
