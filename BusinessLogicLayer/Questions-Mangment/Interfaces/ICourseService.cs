using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.CategoryDto;
using BusinessLogicLayer.Dtos.CourseDto;
using BusinessLogicLayer.Dtos.LectureDto;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseGetDto>> GetCoursesByProfessorAsync(int professorId);
        Task<IEnumerable<CourseGetDto>> GetAllCoursesAsync();
        Task<IEnumerable<CourseGetDto>> GetAllCoursesByCategoryAndProfessorAsync(string categoryName, int professorId);
        Task<CourseGetDto> GetCourseByNameAsync(string courseName);
        Task<CourseGetDto> AddCourseAsync(string name, int categoryId, int professorId);
        Task AddCourseAsync(CourseAddDto courseAddDto);
        Task<CourseGetDto> GetCourseByNameAndProfessorAsync(string name, int professorId);
        Task<CourseGetDto> GetCourseByNameCategoryAndProfessorAsync(string name, int categoryId, int professorId);
        Task<bool> DeleteCourseWithDependenciesAsync(int courseId);
        Task<List<LectureGetDto>> GetLecturesByCourseAsync(int courseId);
        Task<int?> GetCourseIdByNameAndProfessorAsync(string name, int professorId);
        // Task<ProfessorDataDto> GetCoursesAndCategoriesByProfessorAsync(int professorId);


    }
}