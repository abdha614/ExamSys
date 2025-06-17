using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccessLayer.Data;
using DataAccessLayer.Dtos;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        private readonly IMapper _mapper; // Inject the mapper instance

        public ExamRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        //public async Task<List<Exam>> GetByProfessorIdAsync(int professorId)
        //{
        //    var res= await _dbSet.Where(e => e.ProfessorId == professorId)
        //                       .Include(e => e.Course) // Load related Course
        //                       .ToListAsync();
        //    return res;
        //}
        //public async Task<List<ExamWithCourseDto>> GetExamsWithCourseAsync(int professorId)
        //{
        //    var res = await _dbSet.Where(e => e.ProfessorId == professorId)
        //                       .Select(e => new ExamWithCourseDto
        //                       {
        //                           Id = e.Id,
        //                           Title = e.Title,
        //                           CourseName = e.Course.Name, // ✅ Directly get Course Name
        //                           TotalQuestions = e.TotalQuestions
        //                       })
        //                       .ToListAsync();
        //    return res;
        //}
        public async Task<List<ExamWithCourseDto>> GetExamsWithCourseAsync(int professorId)
        {
            // Start the query
            var query = _dbSet.Where(q => q.ProfessorId == professorId).AsQueryable();

            // Project directly into QuestionDto using AutoMapper
            var result = await query
                .ProjectTo<ExamWithCourseDto>(_mapper.ConfigurationProvider)
                 .ToListAsync();

            return result;
        }
        public async Task<ExamDetailDto> GetByIdAndProfessorIdAsync(int examId, int professorId)
        {
            var res = await _dbSet.Where(e => e.Id == examId && e.ProfessorId == professorId)
                               .ProjectTo<ExamDetailDto>(_mapper.ConfigurationProvider) // ✅ AutoMapper handles mapping
                               .FirstOrDefaultAsync();
            return res;
        }

    }

}
