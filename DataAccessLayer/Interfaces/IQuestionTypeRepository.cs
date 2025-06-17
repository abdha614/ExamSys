using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IQuestionTypeRepository : IGenericRepository<QuestionType>
    {
        Task<IEnumerable<QuestionType>> GetAllQuestionTypesAsync(); // Add this method
        Task<QuestionType> GetByNameAsync(string name);
    }
}
