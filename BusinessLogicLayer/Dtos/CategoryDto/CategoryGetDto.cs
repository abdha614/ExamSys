using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class CategoryGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int professorId { get; set; }  // Add the UserId property
    }

   
}

