using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category> GetByNameAsync(string categoryName)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == categoryName);
        }
        public async Task<IEnumerable<Category>> GetByProfessorIdAsync(int professorId)
        {
            return await _context.Categories.Where(c => c.ProfessorId == professorId).ToListAsync();
        }
        public async Task<Category> GetCategoryByNameAndProfessorAsync(string name, int professorId)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name && c.ProfessorId == professorId);
        }
    }
}