using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class QuestionTypeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }  // Assuming "Name" is the property we want to send to the client
    }
}
