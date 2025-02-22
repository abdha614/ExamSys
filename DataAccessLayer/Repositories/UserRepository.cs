using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int? roleId)
        {
            // Start with all users
            var query = _dbSet
                .Include(u => u.Role) // Include the Role entity
                .AsQueryable();

            // If roleId is provided, filter users by that role
            if (roleId.HasValue)
            {
                query = query.Where(u => u.RoleId == roleId.Value);
            }

            // Return the result as a list
            return await query.ToListAsync();
        }

        public async Task DeleteUserAsync(int professorId)
        {
            var categories = await _context.Categories.Where(c => c.ProfessorId == professorId).ToListAsync();
            var courses = await _context.Courses.Where(c => c.ProfessorId == professorId).ToListAsync();

            _context.Categories.RemoveRange(categories);
            _context.Courses.RemoveRange(courses);
            await _context.SaveChangesAsync();

            var professor = await _context.Users.FindAsync(professorId);
            _context.Users.Remove(professor);
            await _context.SaveChangesAsync();
        }
    }

}