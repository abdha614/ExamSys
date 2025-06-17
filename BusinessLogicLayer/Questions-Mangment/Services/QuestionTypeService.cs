using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;

namespace BusinessLogicLayer.Services
{
    public class QuestionTypeService : IQuestionTypeService
    {
        private readonly IQuestionTypeRepository _questionTypeRepository;
        private readonly IMapper _mapper;  // Add AutoMapper

        public QuestionTypeService(IQuestionTypeRepository questionTypeRepository, IMapper mapper)
        {
            _questionTypeRepository = questionTypeRepository;
            _mapper = mapper;
        }

       // Service method to get all question types and return as DTOs
        public async Task<IEnumerable<QuestionTypeDto>> GetAllQuestionTypesAsync()
        {
            var questionTypes = await _questionTypeRepository.GetAllQuestionTypesAsync();

            // Mapping the QuestionType entities to QuestionTypeDto using AutoMapper
            var questionTypeDtos = _mapper.Map<IEnumerable<QuestionTypeDto>>(questionTypes);

            return questionTypeDtos;
        }
        public async Task<int> GetQuestionTypeByNameAsync(string questionTypeName)
        {
            // If questionTypeName is null or empty, return a default Question Type ID (e.g., 1)
            if (string.IsNullOrWhiteSpace(questionTypeName))
            {
                return 1; // Default ID (e.g., "Single Choice" or any basic question type)
            }

            // Query the repository for the question type by name
            var questionType = await _questionTypeRepository.GetByNameAsync(questionTypeName);

            // If the question type doesn't exist, throw an exception
            if (questionType == null)
            {
                throw new ArgumentException($"Question type '{questionTypeName}' not found.");
            }

            // Return the ID of the question type
            return questionType.Id;
        }
        public async Task<List<QuestionTypeDto>> GetQuestionTypesByProfessorIdAsync(int professorId)
        {
            var questionTypes = await _questionTypeRepository.GetAllAsync(); // Await the data before using Where()

            var filteredQuestionTypes = questionTypes
                .Select(qt => new QuestionTypeDto
                {
                    Id = qt.Id,
                    Type = qt.Type
                })
                .ToList();

            return filteredQuestionTypes;
        }



    }
}
