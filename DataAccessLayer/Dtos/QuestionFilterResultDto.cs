using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dtos
{
    public class QuestionFilterResultDto
    {
        public List<Question> Questions { get; set; }
        public List<Category> Categories { get; set; }
        public List<Course> Courses { get; set; }
        public List<Lecture> Lectures { get; set; }
        public List<QuestionType> QuestionTypes { get; set; }
        public List<DifficultyLevel> DifficultyLevels { get; set; }
    }
}
