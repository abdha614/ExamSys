using BusinessLogicLayer.Dtos.LectureDto;
using BusinessLogicLayer.Dtos.QuestionDto;
//using DataAccessLayer.Dtos;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class QuestionFilterDto
    {
        public IEnumerable<QuestionGetDto> Questions { get; set; }
        public List<CategoryGetDto> Categories { get; set; }
        public List<QuestionTypeDto> QuestionTypes { get; set; }
        public List<DifficultyLevelDto> DifficultyLevels { get; set; }
        //public List<CourseGetDto> Courses { get; set; }
        //public List<LectureGetDto> Lectures { get; set; }
        public List<CourseGetDto> Courses { get; set; } = new List<CourseGetDto>();
        public List<LectureGetDto> Lectures { get; set; } = new List<LectureGetDto>();



    }
}
