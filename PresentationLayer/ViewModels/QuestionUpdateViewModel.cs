using BusinessLogicLayer.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{
    public class QuestionUpdateViewModel
    {
        [Required]
        public int Id { get; set; } // Add this property

        [Required]
        public string Text { get; set; }

        [Required]
        public int QuestionTypeId { get; set; }

        [Required]
        public int DifficultyLevelId { get; set; }

        [Required]
        public int professorId { get; set; }

        
        public int? CategoryId { get; set; }

        [Required]
        public int CourseId { get; set; }
        [Required]
        public string LectureName { get; set; } // Instead of LectureId

        public List<AnswerAddViewModel> Answers { get; set; }

        [Required]
        public int CorrectAnswerIndex { get; set; }

        public List<CategoryGetDto> Categories { get; set; } = new List<CategoryGetDto>(); // All available categories
        public List<DifficultyLevelDto> DifficultyLevels { get; set; } = new List<DifficultyLevelDto>();
        public List<CourseGetDto> Courses { get; set; } = new List<CourseGetDto>();       // All available courses

    }
}
