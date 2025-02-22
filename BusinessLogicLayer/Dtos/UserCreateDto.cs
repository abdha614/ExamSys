﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class UserCreateDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; } // Assuming Role is an integer or foreign key to the Role model
    }
}
