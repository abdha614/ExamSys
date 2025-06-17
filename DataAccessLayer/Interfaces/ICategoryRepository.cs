using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Dtos;

namespace DataAccessLayer.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category> GetByNameAsync(string categoryName);
        Task<IEnumerable<Category>> GetByProfessorIdAsync(int professorId);
        Task<int?> GetCategoryIdByNameAndProfessorAsync(string name, int professorId);
        //Task<IEnumerable<Category>> GetAllCategoryWithProfessorEmailAsync();
        Task<IEnumerable<CategoryWithProfessorEmailDto>> GetAllCategoryWithProfessorEmailAsyncs(int? professorId = null);
        Task<bool> RemoveCategoryFromProfessorAsync(int professorId, int categoryId);
        Task<IEnumerable<Category>> GetCategoriesWithCoursesAndLecturesAsync(int professorId);
        Task<IEnumerable<Category>> GetCategoriesWithCoursesByProfessorIdAsync(int professorId);
        Task<List<Course>> GetCoursesByCategoryAsync(int categoryId);
        Task<Category> GetCategoryByNameAndProfessorAsync(string name, int professorId);


    }
}