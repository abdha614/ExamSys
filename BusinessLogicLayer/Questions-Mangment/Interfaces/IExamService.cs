using BusinessLogicLayer.Dtos.ExamDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Questions_Mangment.Interfaces
{
    public interface IExamService
    {
        //Task<ExamGetDto> GetExamByIdAsync(int examId);
        Task AddExamAsync(ExamAddDto examDto);
        Task<List<ExamListDto>> GetExamListAsync(int professorId);
        Task<ExamDetailDtto> GetExamByIdAsync(int examId, int professorId);
        Task DeleteExamAsync(int examId);

        //Task<ExamDetailDto> GetExamByIdAsync(int examId);
        //Task UpdateExamAsync(ExamUpdateDto examDto);
        //Task DeleteExamAsync(int examId);
    }
}
