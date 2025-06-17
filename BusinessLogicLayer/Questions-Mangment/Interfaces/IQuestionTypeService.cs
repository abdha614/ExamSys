
using BusinessLogicLayer.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IQuestionTypeService
    {
        Task<IEnumerable<QuestionTypeDto>> GetAllQuestionTypesAsync();
        Task<int> GetQuestionTypeByNameAsync(string questionTypeName);
        Task<List<QuestionTypeDto>> GetQuestionTypesByProfessorIdAsync(int professorId);
    }
}
    