using DataAccessLayer.Dtos;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IExamRepository : IGenericRepository<Exam>
    {
        Task<List<ExamWithCourseDto>> GetExamsWithCourseAsync(int professorId);
        Task<ExamDetailDto> GetByIdAndProfessorIdAsync(int examId, int professorId);
        

        }
}
