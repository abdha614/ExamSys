using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public bool MustChangePassword { get; set; }
        public string? Password { get; set; } // Optional, only update if provided
    }

}
