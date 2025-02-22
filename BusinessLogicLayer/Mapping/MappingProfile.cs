using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.AnswerDto;
using BusinessLogicLayer.Dtos.CategoryDto;
using BusinessLogicLayer.Dtos.CourseDto;
using BusinessLogicLayer.Dtos.QuestionDto;
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
                 .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<UserRegistrationDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<LoginDto, User>();
            CreateMap<History, HistoryDto>()
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email)); // Map User.Email to HistoryDto.Email

            /////////////////////////////////////

            //Category Mapping
            CreateMap<Category, CategoryGetDto>().ReverseMap();
            CreateMap<Category, CategoryAddDto>().ReverseMap();

            //Course Mapping
            CreateMap<Course, CourseGetDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Course, CourseAddDto>().ReverseMap();

            //Question Mapping
            CreateMap<Question, QuestionGetDto>()
             .ForMember(dest => dest.QuestionTypeName, opt => opt.MapFrom(src => src.QuestionType.Type))
             .ForMember(dest => dest.DifficultyLevelName, opt => opt.MapFrom(src => src.DifficultyLevel.Level))
             .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
             .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
             .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
            CreateMap<Question, QuestionAddDto>().ReverseMap();

            // Answer Mapping
            CreateMap<Answer, AnswerGetDto>().ReverseMap();
            CreateMap<Answer, AnswerAddDto>().ReverseMap();


            CreateMap<QuestionType, QuestionTypeDto>();



        }
    }
}
