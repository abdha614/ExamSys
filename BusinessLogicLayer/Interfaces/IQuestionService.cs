using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.QuestionDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionGetDto> GetQuestionByIdAsync(int questionId);
        Task AddQuestionAsync(QuestionAddDto questionDto);
        Task UpdateQuestionAsync( QuestionUpdateDto questionDto);
        Task DeleteQuestionAsync(int questionId);
        Task<IEnumerable<QuestionGetDto>> GetQuestionsFilteredAsync(int professorId, int? questionTypeId = null, int? difficultyLevelId = null, int? categoryId = null, int? courseId = null);
    }
}
