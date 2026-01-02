using AutoMapper;
using PresentationLayer.ViewModels;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using BusinessLogicLayer.Dtos.QuestionDto;
using BusinessLogicLayer.Dtos.AnswerDto;
using BusinessLogicLayer.Dtos.CategoryDto;
using BusinessLogicLayer.Dtos.CourseDto;
using BusinessLogicLayer.Dtos.ExamDto;
using PresentationLayer.ViewModels.ExamViewModel;
using DataAccessLayer.Dtos;

namespace PresentationLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<QuestionGetDto, QuestionViewModel>();

            CreateMap<RegisterViewModel, UserRegistrationDto>();
            CreateMap<LoginViewModel, LoginDto>();
            CreateMap<RoleDto, RoleViewModel>();
            CreateMap<UserDto, UserViewModel>();
            CreateMap<HistoryDto, HistoryViewModel>();


            // Map from Question to QuestionViewModel
            CreateMap<QuestionGetDto, QuestionViewModel>();

            // Map from QuestionAddViewModel to QuestionAddDto
            CreateMap<QuestionAddViewModel, QuestionAddDto>();



            CreateMap<QuestionFilterDto, QuestionViewModel>();


            CreateMap<QuestionGetDto, ManageQuestionsViewModel>();

            ///////////////
            CreateMap<QuestionGetDto, QuestionAddViewModel>();
            //.ForMember(dest => dest.CorrectAnswerIndex, opt => opt.MapFrom(src => src.Answers.FindIndex(a => a.IsCorrect)));
            CreateMap<QuestionAddViewModel, QuestionGetDto>();

            //  CreateMap<QuestionAddViewModel, QuestionAddDto>();
            // CreateMap<QuestionAddDto, QuestionAddViewModel>();
            ////
            CreateMap<AnswerGetDto, AnswerAddViewModel>();
            // Map from AnswerAddViewModel to AnswerAddDto
            CreateMap<AnswerAddViewModel, AnswerAddDto>();
            //  CreateMap<AnswerAddDto, AnswerAddViewModel>();

            CreateMap<QuestionGetDto, QuestionUpdateViewModel>();
            CreateMap<QuestionUpdateViewModel, QuestionUpdateDto>();


            // Mapping between QuestionImportModel and QuestionImportDto
            //    CreateMap<QuestionImportViewModel, QuestionImportDto>();


            CreateMap<QuestionUpdateDto, QuestionUpdateViewModel>();
            ///////////////////////////////////////////////////////////////////////////
            CreateMap<CategoryGetDto, CategoryViewModel>();

            ///////
            CreateMap<EditProfessorCategoriesViewModel, CategoryAddDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NewCategoryName))
            .ForMember(dest => dest.professorId, opt => opt.MapFrom(src => src.ProfessorId));

            ///////////////////////////////
            CreateMap<AddCourseViewModel, CourseAddDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CourseName))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.SelectedCategoryId))
            .ForMember(dest => dest.professorId, opt => opt.MapFrom(src => src.ProfessorId));

           
            CreateMap<QuestionGetDto, QuestionUpdateViewModel>();
            //////////
            CreateMap<CreateAutoExamViewModel, AutoExamGenerationRequestDto>()
             .ForMember(dest => dest.SelectedLectureIds, opt => opt.MapFrom(src => src.SelectedLectureIds ?? new List<int>()));

            CreateMap<QuestionDto, QuestionViewModel>();

            CreateMap<PreviewExamViewModel, ExamAddDto>();

            CreateMap<ExamDetailDtto, ExamDetailsViewModel>();

            CreateMap<QuestionSettingsViewModel, QuestionSetting>();

            CreateMap<ParsedQuestionsDto, GeneratedQuestionsViewModel>().ReverseMap();
            CreateMap<QuestionGroupDto, GeneratedQuestionViewModel>().ReverseMap();
            CreateMap<QuestionAIDto, QuestionAiViewModel>().ReverseMap();


            CreateMap<SignupNotificationDto, SignupNotificationViewModel>().ReverseMap();




        }


    }
}