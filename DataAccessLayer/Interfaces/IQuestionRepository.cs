using DataAccessLayer.Dtos;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DataAccessLayer.Interfaces
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        Task<IEnumerable<Question>> GetQuestionsByProfessorAsync(int professorId); // List all questions by a professor

        Task<bool> DoesQuestionExistAsync(string questionText, int professorId);

        Task<QuestionDto> GetQuestionByIdAsync(int id); // Get a question by its ID
        Task<Question> GetQuestionByyIdAsync(int id); // Get a question by its ID


        //Task<IEnumerable<Question>> GetQuestionsFilteredAsync(int professorId,
        //    int? questionTypeId = null,
        //    int? difficultyLevelId = null,
        //    int? categoryId = null,
        //    int? courseId = null);
        Task<IEnumerable<QuestionDto>> GetQuestionsFilteredAsync(int professorId,
           int? questionTypeId = null,
           int? difficultyLevelId = null,
           int? categoryId = null,
           int? courseId = null,
           int? lectureId = null);
        Task<IEnumerable<QuestionDto>> GetQuestionsFilteredAdvancedAsync(
           int professorId,
           List<int> questionTypeIds,
           List<int> difficultyLevelIds,
           int? categoryId,
           int? courseId,
           List<int> lectureIds);
        Task<List<Question>> GetQuestionsByIdsAsync(List<int> questionIds);
        Task DeleteQuestionAndAnswersAsync(int questionId);
        //    Task<List<Question>> GetFilteredQuestionsAsync(
        //int professorId,
        //int? selectedCategoryId,
        //int? selectedCourseId,
        //List<int>? selectedLectureIds,
        //int easyCount,
        //int mediumCount,
        //int hardCount,
        //int mcqCount,
        //int tfCount);
        Task<List<QuestionDto>> GetQuestionsByTypeAndDifficultyAsync(
            string type,
            string difficulty,
            int count,
            int professorId,
            int? courseId,
            int? categoryId,
            List<int> lectureIds
        );

    }

}
