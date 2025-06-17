using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class RegisteredUserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }

    }
}
