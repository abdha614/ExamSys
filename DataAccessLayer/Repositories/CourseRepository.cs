using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAndProfessorAsync(string categoryName, int professorId)
        {
            return await _dbSet
                .Include(c => c.Category)  // Load Category
                .Where(c => c.Category.Name == categoryName && c.ProfessorId == professorId)  // Filter by Category and professorId
                .ToListAsync();
        }

        public async Task<Course> GetByNameAsync(string courseName)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == courseName);
        }
        public async Task<IEnumerable<Course>> GetByProfessorIdAsync(int professorId)
        {
            return await _context.Courses.Where(c => c.ProfessorId == professorId).ToListAsync();
        }
        public async Task<Course> GetCourseByNameAndProfessorAsync(string name, int professorId)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.Name == name && c.ProfessorId == professorId); 
        }
        public async Task<Course> GetCourseByNameCategoryAndProfessorAsync(string name, int categoryId, int professorId)
        {
            return await _context.Courses
                                 .FirstOrDefaultAsync(c => c.Name == name && c.CategoryId == categoryId && c.ProfessorId == professorId);
        }
    }
}
