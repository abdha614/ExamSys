﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class CategoryWithCoursesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CourseGetDto> Courses { get; set; }
    }

}
