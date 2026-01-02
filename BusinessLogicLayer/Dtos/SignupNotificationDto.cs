using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class SignupNotificationDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime RequestedAt { get; set; }
        public bool IsHandled { get; set; }
    }
}
