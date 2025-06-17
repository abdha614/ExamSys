using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.CourseDto
{
    public class CourseAddDto
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int professorId { get; set; }

    }
}
