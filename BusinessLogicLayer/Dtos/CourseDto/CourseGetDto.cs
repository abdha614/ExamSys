using BusinessLogicLayer.Dtos.LectureDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class CourseGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // Enriched with the corresponding name

        public List<LectureGetDto> Lectures { get; set; }

        //   public int professorId { get; set; }  // Add the UserId property
        // public CategoryGetDto Category { get; set; }
    }

    
}

