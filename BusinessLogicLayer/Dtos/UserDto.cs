using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; internal set; }
        public bool MustChangePassword { get; set; }

    }
}


