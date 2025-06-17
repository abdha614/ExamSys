using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.AnswerDto;
using BusinessLogicLayer.Dtos.ExamDto;
using BusinessLogicLayer.Dtos.QuestionDto;
using DataAccessLayer.Dtos;
using DataAccessLayer.Models;
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
        Task<bool> DoesQuestionExistAsync(string questionText, int professorId);
        Task<QuestionFilterDto> GetQuestionsFilteredAsync(int professorId, int? questionTypeId = null, int? difficultyLevelId = null, int? categoryId = null, int? courseId = null, int? lectureId = null);

        //Task<IEnumerable<QuestionGetDto>> GetQuestionsFilteredAsync(int professorId, int? questionTypeId = null, int? difficultyLevelId = null, int? categoryId = null, int? courseId = null);

        Task<QuestionFilterDto> GetFilterOptionsAsync( int professorId);
        Task<QuestionFilterDto> GetQuestionsFilteredAdvancedAsync(
           int professorId,
           List<int> questionTypeIds,
           List<int> difficultyLevelIds,
           int? categoryId,
           int? courseId,
           List<int> lectureIds);


        Task<List<AnswerGetDto>> GetAnswersForQuestionsAsync(List<int> questionIds);
        //Task<QuestionAutoFilterDto> GenerateQuestionsBasedOnPercentagesAsync(AutoExamGenerationRequestDto requestDto);
        Task<QuestionFilterDto> GenerateQuestionsBasedOnCountsAsync(AutoExamGenerationRequestDto requestDto);

    }
}
