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
    }
}
