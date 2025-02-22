using AutoMapper;
using PresentationLayer.ViewModels;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using BusinessLogicLayer.Dtos.QuestionDto;
using BusinessLogicLayer.Dtos.AnswerDto;
using BusinessLogicLayer.Dtos.CategoryDto;
using BusinessLogicLayer.Dtos.CourseDto;

namespace PresentationLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, UserRegistrationDto>();
            CreateMap<LoginViewModel, LoginDto>();
            CreateMap<RoleDto, RoleViewModel>();
            CreateMap<UserDto, UserViewModel>();
            CreateMap<HistoryDto, HistoryViewModel>();
            ////////////////////
            //   CreateMap<QuestionDto, QuestionViewModel>();

            // Mapping from MultipleChoiceQuestionViewModel to MultipleChoiceQuestionDto
            //CreateMap<MultipleChoiceQuestionViewModel, MultipleChoiceQuestionDto>()
            //    .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
            //    .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.QuestionType))
            //    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
            //    .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseName))
            //    .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel))
            //    .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
            //    CreateMap<QuestionDto, QuestionViewModel>().ReverseMap();
            //   CreateMap<AnswerDto, AnswerViewModel>().ReverseMap();

            // Map from Question to QuestionViewModel
            CreateMap<QuestionGetDto, QuestionViewModel>();

            // Map from QuestionAddViewModel to QuestionAddDto
            CreateMap<QuestionAddViewModel, QuestionAddDto>();

            
            ///////////////
            CreateMap<QuestionGetDto, QuestionAddViewModel>()
                .ForMember(dest => dest.CorrectAnswerIndex, opt => opt.MapFrom(src => src.Answers.FindIndex(a => a.IsCorrect)));
            CreateMap<QuestionAddViewModel, QuestionGetDto>();

            CreateMap<QuestionAddViewModel, QuestionAddDto>();
            CreateMap<QuestionAddDto, QuestionAddViewModel>();
            ////
            CreateMap<AnswerGetDto, AnswerAddViewModel>();
            CreateMap<AnswerAddViewModel, AnswerGetDto>();
            // Map from AnswerAddViewModel to AnswerAddDto
            CreateMap<AnswerAddViewModel, AnswerAddDto>();
            CreateMap<AnswerAddDto, AnswerAddViewModel>();

            CreateMap<QuestionGetDto, QuestionUpdateViewModel>();
            CreateMap<QuestionUpdateViewModel, QuestionUpdateDto>();


            // Mapping between QuestionImportModel and QuestionImportDto
            CreateMap<QuestionImportViewModel, QuestionImportDto>();

            CreateMap<QuestionUpdateViewModel, QuestionUpdateDto>();
            CreateMap<QuestionUpdateDto, QuestionUpdateViewModel>();

        }
    }
}