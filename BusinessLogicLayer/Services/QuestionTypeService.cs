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
    }
}
