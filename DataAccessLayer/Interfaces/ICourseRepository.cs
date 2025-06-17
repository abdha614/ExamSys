using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<IEnumerable<Course>> GetCoursesByCategoryAndProfessorAsync(string categoryName, int professorId);
        Task<Course> GetByNameAsync(string courseName);
        Task<IEnumerable<Course>> GetByProfessorIdAsync(int professorId);
        Task<Course> GetCourseByNameAndProfessorAsync(string name, int professorId);
        Task<Course> GetCourseByNameCategoryAndProfessorAsync(string name, int categoryId, int professorId);
        Task<IEnumerable<Course>> GetCoursesWithCategoriesByProfessorAsync(int professorId);
        Task<bool> DeleteCourseWithDependenciesAsync(int courseId);
        Task<List<Lecture>> GetLecturesByCourseAsync(int courseId);
        Task<int?> GetCourseIdByNameAndProfessorAsync(string name, int professorId);

    }
}
