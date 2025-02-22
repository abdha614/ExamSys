
using BusinessLogicLayer.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IQuestionTypeService
    {
        Task<IEnumerable<QuestionTypeDto>> GetAllQuestionTypesAsync();
    }
}
