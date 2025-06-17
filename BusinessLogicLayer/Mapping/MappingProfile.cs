using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.AnswerDto;
using BusinessLogicLayer.Dtos.CategoryDto;
using BusinessLogicLayer.Dtos.CourseDto;
using BusinessLogicLayer.Dtos.ExamDto;
using BusinessLogicLayer.Dtos.QuestionDto;
using DataAccessLayer.Dtos;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                 .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Roles.Name));

            CreateMap<UserRegistrationDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<LoginDto, User>();
            CreateMap<History, HistoryDto>()
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email)); // Map User.Email to HistoryDto.Email

            /////////////////////////////////////

            //Category Mapping
            CreateMap<Category, CategoryGetDto>()
                              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Professor.Email)); // Map User.Email to HistoryDto.Email



            CreateMap<CategoryAddDto, Category>();

            //Course Mapping
            CreateMap<Course, CourseGetDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Course, CourseAddDto>().ReverseMap();

            //Question Mapping
            CreateMap<QuestionDto, QuestionGetDto>();
            // .ForMember(dest => dest.QuestionTypeName, opt => opt.MapFrom(src => src.QuestionType.Type))
            // .ForMember(dest => dest.DifficultyLevelName, opt => opt.MapFrom(src => src.DifficultyLevel.Level))
            // .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            // .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
            // .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
            CreateMap<Question, QuestionAddDto>().ReverseMap();

            // Answer Mapping
            CreateMap<AnswerGettDto, AnswerGetDto>().ReverseMap();
            CreateMap<AnswerAddDto, Answer>();

            CreateMap<DifficultyLevel, DifficultyLevelDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Level));


            CreateMap<QuestionType, QuestionTypeDto>();

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            CreateMap<CategoryWithProfessorEmailDto, CategoryGetDto>()
                             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ProfessorEmail)) // Map User.Email to HistoryDto.Email
                             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName));
            //////
            CreateMap<Answer, AnswerGetDto>().ReverseMap();

            CreateMap<QuestionGetDto, Question>();

            CreateMap<Question, QuestionGetDto>();


            CreateMap<ExamAddDto, Exam>()
        .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ExamTitle)) // Map ExamTitle to Title
        .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.SelectedCourseId ?? 0)); // Map SelectedCourseId with a default value

            CreateMap<ExamWithCourseDto, ExamListDto>();
            //
            CreateMap<ExamDetailDto, ExamDetailDtto>();

            CreateMap<ExamQuestionDto, ExamQuestionDtto>();
            CreateMap<AnswerGettDto, AnswerGettDtto>();
            CreateMap<Category, CategoryGetDto>()
               .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Courses));




        }
    }
}
