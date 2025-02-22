using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        Task<IEnumerable<Question>> GetQuestionsByProfessorAsync(int professorId); // List all questions by a professor
        Task<Question> GetQuestionByIdAsync(int id); // Get a question by its ID
      
        Task<IEnumerable<Question>> GetQuestionsFilteredAsync(int professorId,
            int? questionTypeId = null,
            int? difficultyLevelId = null,
            int? categoryId = null,
            int? courseId = null);

       
    }

}
