using AutoMapper;
using DataAccessLayer.Dtos;
using DataAccessLayer.Migrations;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Question, QuestionDto>()
              .ForMember(dest => dest.QuestionTypeName, opt => opt.MapFrom(src => src.QuestionType.Type))
              .ForMember(dest => dest.DifficultyLevelName, opt => opt.MapFrom(src => src.DifficultyLevel.Level))
              .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
              .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
              .ForMember(dest => dest.LectureName, opt => opt.MapFrom(src => src.Lecture.LectureName)); // Assuming Lecture.Name exists

            CreateMap<Category, CategoryWithProfessorEmailDto>()
              .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.ProfessorEmail, opt => opt.MapFrom(src => src.Professor.Email));

            CreateMap<Answer, AnswerGettDto>().ReverseMap();

            CreateMap<Exam, ExamWithCourseDto>()
             .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name));
            /////////////

            CreateMap<Exam, ExamDetailDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ExamQuestions));

            CreateMap<ExamQuestion, ExamQuestionDto>()
              .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Question.Answers)) // ✅ Map answers
              .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.Question.QuestionType.Type))
              .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question.Text));

            CreateMap<Answer, AnswerGettDto>();


        }
    }

}
