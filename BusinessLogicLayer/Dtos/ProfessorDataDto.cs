using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class ProfessorDataDto
    {
        public List<CategoryGetDto> Categories { get; set; }

        public List<DifficultyLevelDto> DifficultyLevels { get; set; }

        public List<CourseGetDto> Courses { get; set; } 
    }

}
