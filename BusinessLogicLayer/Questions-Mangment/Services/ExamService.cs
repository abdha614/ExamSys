using AutoMapper;
using BusinessLogicLayer.Dtos.ExamDto;
using BusinessLogicLayer.Questions_Mangment.Interfaces;
using DataAccessLayer.Dtos;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Migrations;
using DataAccessLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;

        public ExamService(IExamRepository examRepository, IMapper mapper)
        {
            _examRepository = examRepository;
            _mapper = mapper;
        }

        //public async Task<ExamGetDto> GetExamByIdAsync(int examId)
        //{
        //    var exam = await _examRepository.GetExamByIdAsync(examId);
        //    return _mapper.Map<ExamGetDto>(exam);
        //}

        public async Task AddExamAsync(ExamAddDto examDto)
        {
            var exam = _mapper.Map<Exam>(examDto);

            var metadata = new
            {
                CourseName = examDto.CourseNamee,
                Semester = examDto.Semester,
                TeacherName = examDto.TeacherName
            };
            // Create ExamQuestion entries for selected questions
            foreach (var question in examDto.SelectedQuestions)
            {
                var examQuestion = new ExamQuestion
                {
                    ExamId = exam.Id,
                    QuestionId = question.QuestionId,
                    Order = question.OrderIndex,
                    Points = question.Points
                };
                exam.ExamQuestions.Add(examQuestion);
            }
            exam.Metadata = JsonConvert.SerializeObject(metadata); // Store as JSON
            exam.TotalQuestions = examDto.SelectedQuestions.Count;

            await _examRepository.AddAsync(exam);
        }

        //public async Task UpdateExamAsync(ExamUpdateDto examDto)
        //{
        //    var exam = _mapper.Map<Exam>(examDto);
        //    await _examRepository.UpdateAsync(exam);
        //}

        //public async Task DeleteExamAsync(int examId)
        //{
        //    await _examRepository.DeleteAsync(examId);
        //} 
        public async Task<List<ExamListDto>> GetExamListAsync(int professorId)
        {
            var exams = await _examRepository.GetExamsWithCourseAsync(professorId); // Fetch exams by professor

            return _mapper.Map<List<ExamListDto>>(exams); // ✅ Direct mapping
        }

        //public async Task<ExamDetailDto> GetExamByIdAsync(int examId)
        //{
        //    var exam = await _examRepository.GetByIdAsync(examId);

        //    if (exam == null) return null;

        //    return new ExamDetailDto
        //    {
        //        Id = exam.Id,
        //        Title = exam.Title,
        //        Course = JsonConvert.DeserializeObject<dynamic>(exam.Metadata).CourseName,
        //        TotalQuestions = exam.TotalQuestions,
        //        Questions = exam.ExamQuestions.Select(q => new QuestionDto
        //        {
        //            QuestionId = q.QuestionId,
        //            Order = q.Order,
        //            Points = q.Points
        //        }).ToList()
        //    };
        //}
        //public async Task<ExamDetailDtto> GetExamByIdAsync(int examId, int professorId)
        //{
        //    var exam = await _examRepository.GetByIdAndProfessorIdAsync(examId, professorId);

        //    return _mapper.Map<ExamDetailDtto>(exam);

        //}
        public async Task<ExamDetailDtto> GetExamByIdAsync(int examId, int professorId)
        {
            var exam = await _examRepository.GetByIdAndProfessorIdAsync(examId, professorId);

            if (exam == null)
            {
                throw new Exception("Exam not found."); // ✅ Ensures valid data
            }

            // ✅ Extract metadata
            var metadata = JsonConvert.DeserializeObject<dynamic>(exam.Metadata);

            var examDto = _mapper.Map<ExamDetailDtto>(exam);

            // ✅ Assign extracted metadata values
            examDto.CourseName = metadata.CourseName;
            examDto.Semester = metadata.Semester;
            examDto.TeacherName = metadata.TeacherName;

            return examDto;
        }
        public async Task DeleteExamAsync(int examId)
        {
            await _examRepository.DeleteAsync(examId);
        }


    }
}