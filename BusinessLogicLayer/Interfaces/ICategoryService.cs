using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.CategoryDto;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryGetDto>> GetAllCategoriesAsync();
        Task<CategoryGetDto> GetCategoryByNameAsync(string categoryName);
        Task<CategoryGetDto> GetCategoryByIdAsync(int categoryId); // Add this method
        Task<CategoryGetDto> AddCategoryAsync(string name, int professorId);
        Task<IEnumerable<CategoryGetDto>> GetCategoriesByProfessorAsync(int professorId);

        Task<CategoryGetDto> GetCategoryByNameAndProfessorAsync(string name, int professorId);

    }
}
