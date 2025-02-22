using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Question>> GetQuestionsFilteredAsync(int professorId,
     int? questionTypeId = null,
     int? difficultyLevelId = null,
     int? categoryId = null,
     int? courseId = null)
        {
            // Start the query filtered by professorId
            var query = _dbSet
                .Where(q => q.ProfessorId == professorId) // Filter by professor ID
                .Include(q => q.QuestionType)         // Eagerly load related QuestionType
                .Include(q => q.DifficultyLevel)      // Eagerly load related DifficultyLevel
                .Include(q => q.Category)             // Eagerly load related Category
                .Include(q => q.Course)               // Eagerly load related Course
                .AsQueryable();

            // Apply the filters incrementally based on provided parameters

            if (questionTypeId.HasValue)
            {
                // Apply questionTypeId filter
                query = query.Where(q => q.QuestionTypeId == questionTypeId.Value);
            }

            if (difficultyLevelId.HasValue)
            {
                // Apply difficultyLevelId filter to the already filtered query
                query = query.Where(q => q.DifficultyLevelId == difficultyLevelId.Value);
            }

            if (categoryId.HasValue)
            {
                // Apply categoryId filter to the already filtered query
                query = query.Where(q => q.CategoryId == categoryId.Value);
            }

            if (courseId.HasValue)
            {
                // Apply courseId filter to the already filtered query
                query = query.Where(q => q.CourseId == courseId.Value);
            }

            // Execute the query and return the filtered results
            return await query.ToListAsync();
        }



        public async Task<IEnumerable<Question>> GetQuestionsByProfessorAsync(int professorId)
        {
            return await _dbSet
                .Where(q => q.ProfessorId == professorId)    // Filter by professor
                .Include(q => q.QuestionType)            // Eagerly load the related QuestionType
                .Include(q => q.DifficultyLevel)         // Eagerly load the related DifficultyLevel
                .Include(q => q.Category)                // Eagerly load the related Category
                .Include(q => q.Course)                  // Eagerly load the related Course
                .ToListAsync();                          // Execute the query asynchronously
        }
        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.QuestionType)
                .Include(q => q.DifficultyLevel)
                .Include(q => q.Category)
                .Include(q => q.Course)
                .Include(q => q.Answers) // Include answers
                .FirstOrDefaultAsync(q => q.Id == id);
        }
      
        
    }
}