using DataAccessLayer.Data;
using DataAccessLayer.Dtos;
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
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int? roleId)
        {
            // Start with all users
            var query = _dbSet
                .Include(u => u.Roles) // Include the Role entity
                .Include(u => u.Categories)
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
            // Step 1: Remove related categories
            var categories = await _context.Categories
                .Where(c => c.ProfessorId == professorId)
                .ToListAsync();
            _context.Categories.RemoveRange(categories);

            // Step 2: Remove related courses
            var courses = await _context.Courses
                .Where(c => c.ProfessorId == professorId)
                .ToListAsync();
            _context.Courses.RemoveRange(courses);

            // Step 3: Remove related histories
            var histories = await _context.Histories
                .Where(h => h.UserId == professorId)
                .ToListAsync();
            _context.Histories.RemoveRange(histories);

            // Save changes for related entities
            await _context.SaveChangesAsync();

            // Step 4: Delete the professor
            var professor = await _context.Users.FindAsync(professorId);
            _context.Users.Remove(professor);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }
        public async Task<CreatedUserDto> AddUserAsync(User user)
        {
            await _dbSet.AddAsync(user);
            await _context.SaveChangesAsync();

            return new CreatedUserDto
            {
                Id = user.Id,
                RoleId = user.RoleId
            };
        }

    }

}