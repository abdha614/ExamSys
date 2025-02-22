using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos.QuestionDto;
using DataAccessLayer.Models;

namespace BusinessLogicLayer.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepository, IAnswerRepository answerRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _mapper = mapper;
        }
        public async Task<QuestionGetDto> GetQuestionByIdAsync(int questionId)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionId);
            return _mapper.Map<QuestionGetDto>(question);
        }
        public async Task AddQuestionAsync(QuestionAddDto questionDto)
        {
            // Map question DTO to entity
            var question = _mapper.Map<Question>(questionDto);

            // Add the question to the repository
            await _questionRepository.AddAsync(question);
        }

        public async Task UpdateQuestionAsync( QuestionUpdateDto questionDto)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionDto.Id);
            if (question == null)
            {
                throw new KeyNotFoundException("Question not found");
            }

            // Update the question entity with values from the DTO
            question.Text = questionDto.Text;
            question.CategoryId = questionDto.CategoryId;
            question.CourseId = questionDto.CourseId;
            question.DifficultyLevelId = questionDto.DifficultyLevelId;
            question.Answers = _mapper.Map<List<Answer>>(questionDto.Answers);

            await _questionRepository.UpdateAsync(question);
        }


        public async Task DeleteQuestionAsync(int questionId)
        {
            await _questionRepository.DeleteAsync(questionId);
        }
        public async Task<IEnumerable<QuestionGetDto>> GetQuestionsFilteredAsync(int professorId,
             int? questionTypeId = null,
             int? difficultyLevelId = null,
             int? categoryId = null,
             int? courseId = null)
        {
            // Call the repository to fetch the filtered questions based on the provided filters
            var questions = await _questionRepository.GetQuestionsFilteredAsync(professorId, questionTypeId, difficultyLevelId, categoryId, courseId);

            // Map the result to a DTO (Data Transfer Object) format
            return _mapper.Map<IEnumerable<QuestionGetDto>>(questions);
        }

    }
}
