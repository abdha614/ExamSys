using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.CategoryDto;
using DataAccessLayer.Dtos;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryGetDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryGetDto>> GetAllCategoryWithProfessorEmailAsyncs(int? professorId = null);
        Task<CategoryGetDto> GetCategoryByNameAsync(string categoryName);
        Task<CategoryGetDto> GetCategoryByIdAsync(int categoryId); // Add this method
        Task AddCategoryAsync(CategoryAddDto categoryAddDto);
        Task<IEnumerable<CategoryGetDto>> GetCategoriesByProfessorAsync(int professorId);
        Task<int?> GetCategoryIdByNameAndProfessorAsync(string name, int professorId);
        Task RemoveCategoryFromProfessorAsync(int professorId, int categoryId);
        Task<ProfessorDataDto> GetAllRelatedDataForProfessorAsync(int professorId);
        Task<IEnumerable<CategoryWithCoursesDto>> GetCategoriesWithCoursesByProfessorIdAsync(int professorId);
        Task<List<CourseGetDto>> GetCoursesWithQuestionsByCategoryAsync(int categoryId);
        Task<List<CategoryGetDto>> GetCategoriesByProfessorIdAsync(int professorId);
        Task<CategoryGetDto> GetCategoryByNameAndProfessorAsync(string name, int professorId);


        }
}
