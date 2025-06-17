using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class QuestionTypeRepository : GenericRepository<QuestionType>, IQuestionTypeRepository
    {
        public QuestionTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implement GetAllQuestionTypesAsync method to fetch all QuestionTypes
        public async Task<IEnumerable<QuestionType>> GetAllQuestionTypesAsync()
        {
            return await _dbSet.ToListAsync(); // Fetch all QuestionTypes
        }
        public async Task<QuestionType> GetByNameAsync(string name)
        {
            string normalizedInput = name.Replace(" ", "").Replace("-", "").Trim().ToLower();

            return await _dbSet
                .FirstOrDefaultAsync(qt =>
                    qt.Type.Replace(" ", "").Replace("-", "").Trim().ToLower() == normalizedInput);
        }

    }
}
