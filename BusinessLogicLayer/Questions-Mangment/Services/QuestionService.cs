using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos.QuestionDto;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using DataAccessLayer.Dtos;
using BusinessLogicLayer.Dtos.LectureDto;
using BusinessLogicLayer.Dtos.AnswerDto;
using DocumentFormat.OpenXml.Office2013.Excel;
using BusinessLogicLayer.Dtos.ExamDto;
using System.Text.RegularExpressions;

namespace BusinessLogicLayer.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly ICourseService _courseService;
        private IQuestionTypeRepository _questionTypeRepository;
        private IDifficultyLevelRepository _difficultyLevelRepository;
        private ILectureRepository _lectureRepository;
        private IQuestionTypeService _questionTypeService;
        private IDifficultyLevelService _difficultyLevelService;

        public QuestionService(IQuestionRepository questionRepository, IAnswerRepository answerRepository, IDifficultyLevelRepository difficultyLevelRepository, IDifficultyLevelService difficultyLevelService, IQuestionTypeService questionTypeService
            , ICourseService courseService, ICategoryService categoryService, ILectureRepository lectureRepository, IMapper mapper, IQuestionTypeRepository questionTypeRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _mapper = mapper;
            _categoryService = categoryService;
            _courseService = courseService;
            _questionTypeRepository = questionTypeRepository;
            _difficultyLevelRepository = difficultyLevelRepository;
            _lectureRepository = lectureRepository;
            _questionTypeService = questionTypeService;
            _difficultyLevelService = difficultyLevelService;

        }
        public async Task<QuestionGetDto> GetQuestionByIdAsync(int questionId)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionId);
            return _mapper.Map<QuestionGetDto>(question);
        }
        public async Task AddQuestionAsync(QuestionAddDto questionDto)
        {
            // Get or create the lecture
            var lecture = await _lectureRepository.GetByIdAsync(
                questionDto.LectureName,
                questionDto.CourseId,
                questionDto.professorId);

            if (lecture == null || lecture.CourseId != questionDto.CourseId || lecture.ProfessorId != questionDto.professorId)
            {
                lecture = new Lecture
                {
                    LectureName = questionDto.LectureName,
                    CourseId = questionDto.CourseId,
                    ProfessorId = questionDto.professorId
                };

                await _lectureRepository.AddAsync(lecture);
            }

            questionDto.LectureId = lecture.Id;

            // ✅ Duplicate check using repository method with 3 parameters
            bool questionExists = await _questionRepository.DoesQuestionExistAsync(
                   questionDto.Text,
                   questionDto.LectureId,
                   questionDto.professorId);

            // 👉 If the question does NOT exist, add it
            if (!questionExists)
            {
                var question = _mapper.Map<Question>(questionDto);
                await _questionRepository.AddAsync(question);
            }
            // 👉 If it already exists, just continue with the rest of the method
            // (no exception thrown, no duplicate insert)

        }





        public async Task UpdateQuestionAsync(QuestionUpdateDto questionDto)
        {
            // Retrieve the question from the database
            var question = await _questionRepository.GetQuestionByyIdAsync(questionDto.Id);
            if (question == null)
            {
                throw new KeyNotFoundException("Question not found");
            }

            // Retrieve the LectureId using LectureName
            var lecture = await _lectureRepository.GetByIdAsync(questionDto.LectureName, questionDto.CourseId,questionDto.professorId);
            if (lecture == null)
            {
                lecture = new Lecture
                {
                    //  Id = questionDto.LectureId, // Use the provided LectureId
                    LectureName = questionDto.LectureName, // Generate name dynamically
                    CourseId = questionDto.CourseId, // Link to the appropriate course
                    ProfessorId = questionDto.professorId // Set the professor ID
                };

                // Add the new lecture to the database
                await _lectureRepository.AddAsync(lecture);
            }
            // Update the question entity with values from the DTO
            question.Text = questionDto.Text;
            question.CategoryId = questionDto.CategoryId;
            question.CourseId = questionDto.CourseId;
            question.DifficultyLevelId = questionDto.DifficultyLevelId;
            question.LectureId = lecture.Id; // Assign the LectureId
            question.Answers = _mapper.Map<List<Answer>>(questionDto.Answers);

            await _questionRepository.UpdateAsync(question);
        }



        public async Task DeleteQuestionAsync(int questionId)
        {
            await _questionRepository.DeleteQuestionAndAnswersAsync(questionId);
        }
        public async Task<bool> DoesQuestionExistAsync(string questionText, int professorId)
        {
            return await _questionRepository.DoesQuestionExistAsync(questionText, professorId);
        }

        public async Task<QuestionFilterDto> GetQuestionsFilteredAsync(int professorId,
    int? questionTypeId = null,
    int? difficultyLevelId = null,
    int? categoryId = null,
    int? courseId = null,
    int? lectureId = null)

        {
            var questionDtos = await _questionRepository.GetQuestionsFilteredAsync(professorId, questionTypeId, difficultyLevelId, categoryId, courseId, lectureId);
            var result = _mapper.Map<IEnumerable<QuestionGetDto>>(questionDtos);

            // Extract filter lists from existing data
            var categories = questionDtos
                .DistinctBy(q => q.CategoryId)
                .Select(q => new CategoryGetDto { Id = q.CategoryId, Name = q.CategoryName })
                .ToList();

            var questionTypes = questionDtos
                .DistinctBy(q => q.QuestionTypeId)
                .Select(q => new QuestionTypeDto { Id = q.QuestionTypeId, Type = q.QuestionTypeName })
                .ToList();

            var difficultyLevels = questionDtos
                .DistinctBy(q => q.DifficultyLevelId)
                .Select(q => new DifficultyLevelDto { Id = q.DifficultyLevelId, Name = q.DifficultyLevelName })
                .ToList();

            var courses = questionDtos
                .DistinctBy(q => q.CourseId)
                .Select(q => new CourseGetDto { Id = q.CourseId, Name = q.CourseName, CategoryId = q.CategoryId, CategoryName = q.CategoryName })
                .ToList();

            return new QuestionFilterDto
            {
                Questions = result,
                Categories = categories,
                QuestionTypes = questionTypes,
                DifficultyLevels = difficultyLevels,
                Courses = courses
            };
        }
        public async Task<QuestionFilterDto> GetFilterOptionsAsync(
     int professorId)
        {
            var questionDtos = await _questionRepository.GetQuestionsFilteredAsync(professorId);
            //var result = _mapper.Map<IEnumerable<QuestionGetDto>>(questionDtos);

            // Extract filter lists from existing data
            var categories = questionDtos
                .DistinctBy(q => q.CategoryId)
                .Select(q => new CategoryGetDto { Id = q.CategoryId, Name = q.CategoryName })
                .ToList();

            var questionTypes = questionDtos
                .DistinctBy(q => q.QuestionTypeId)
                .Select(q => new QuestionTypeDto { Id = q.QuestionTypeId, Type = q.QuestionTypeName })
                .ToList();

            var difficultyLevels = questionDtos
                .DistinctBy(q => q.DifficultyLevelId)
                .Select(q => new DifficultyLevelDto { Id = q.DifficultyLevelId, Name = q.DifficultyLevelName })
                .ToList();

            var courses = questionDtos
                .DistinctBy(q => q.CourseId)
                .Select(q => new CourseGetDto { Id = q.CourseId, Name = q.CourseName, CategoryId = q.CategoryId, CategoryName = q.CategoryName })
                .ToList();
            // Fetch filtered questions from repository
            //var questionDtos = await _questionRepository.GetQuestionsFilteredAdvancedAsync(
            //    professorId, questionTypeIds, difficultyLevelIds, categoryId, courseId, lectureIds);

            //var result = _mapper.Map<IEnumerable<QuestionGetDto>>(questionDtos);

            //  var categories = await _categoryService.GetCategoriesByProfessorIdAsync(professorId);

            //var questionTypes = questionDtos
            //    .DistinctBy(q => q.QuestionTypeId)
            //    .Select(q => new QuestionTypeDto { Id = q.QuestionTypeId, Type = q.QuestionTypeName })
            //    .ToList();
            //   var questionTypes = await _questionTypeService.GetQuestionTypesByProfessorIdAsync(professorId);

            //var difficultyLevels = questionDtos
            //    .DistinctBy(q => q.DifficultyLevelId)
            //    .Select(q => new DifficultyLevelDto { Id = q.DifficultyLevelId, Name = q.DifficultyLevelName })
            //    .ToList();
            //   var difficultyLevels = await _difficultyLevelService.GetDifficultyLevelsByProfessorIdAsync(professorId);

            //var courses = questionDtos
            //    .DistinctBy(q => q.CourseId)
            //    .Select(q => new CourseGetDto { Id = q.CourseId, Name = q.CourseName, CategoryId = q.CategoryId, CategoryName = q.CategoryName })
            //    .ToList();
            var lectures = questionDtos
                .DistinctBy(q => q.LectureId)
                .Select(q => new LectureGetDto { Id = q.LectureId, LectureName = q.LectureName })
                .ToList();


            return new QuestionFilterDto
            {
                //Questions = result,
                Categories = categories,
                QuestionTypes = questionTypes,
                DifficultyLevels = difficultyLevels,
                Courses = courses,
                Lectures = lectures

            };
        }
        public async Task<QuestionFilterDto> GetQuestionsFilteredAdvancedAsync(
     int professorId,
     List<int> questionTypeIds,
     List<int> difficultyLevelIds,
     int? categoryId,
     int? courseId,
     List<int> lectureIds)
        {
            // Fetch filtered questions from repository
            var questionDtos = await _questionRepository.GetQuestionsFilteredAdvancedAsync(
                professorId, questionTypeIds, difficultyLevelIds, categoryId, courseId, lectureIds);

            var result = _mapper.Map<IEnumerable<QuestionGetDto>>(questionDtos);

            return new QuestionFilterDto
            {
                Questions = result
            };
        }

        public async Task<List<AnswerGetDto>> GetAnswersForQuestionsAsync(List<int> questionIds)
        {
            var questions = await _questionRepository.GetQuestionsByIdsAsync(questionIds);

            return questions.SelectMany(q => q.Answers.Select(a => new AnswerGetDto
            {
                QuestionId = q.Id,  // Ensuring the answer is mapped to the correct question
                Text = a.Text,
                IsCorrect = a.IsCorrect
            })).ToList();
        }
        public async Task<int> CountQuestionsAsync(
            int professorId,
            int courseId,
            int? categoryId,
            List<int> lectureIds,
            int questionTypeId,
            int difficultyId)
        {
            // Simply delegate to the repository
            return await _questionRepository.CountQuestionsAsync(
                professorId,
                courseId,
                categoryId,
                lectureIds,
                questionTypeId,
                difficultyId
            );
        }
        public async Task<QuestionFilterDto> GenerateQuestionsBasedOnCountsAsync(AutoExamGenerationRequestDto requestDto)
        {
            var allSelectedQuestions = new List<QuestionDto>();

            var professorId = requestDto.ProfessorId;
            var courseId = requestDto.SelectedCourseId;
            var categoryId = requestDto.SelectedCategoryId;
            var lectureIds = requestDto.SelectedLectureIds ?? new List<int>();

            // MCQ
            allSelectedQuestions.AddRange(await _questionRepository.GetAutoQuestionsFilteredAsync(
                "Multiple Choice", "Easy", requestDto.McqEasy, professorId, courseId, categoryId, lectureIds));

            allSelectedQuestions.AddRange(await _questionRepository.GetAutoQuestionsFilteredAsync(
                "Multiple Choice", "Medium", requestDto.McqMedium, professorId, courseId, categoryId, lectureIds));

            allSelectedQuestions.AddRange(await _questionRepository.GetAutoQuestionsFilteredAsync(
                "Multiple Choice", "Hard", requestDto.McqHard, professorId, courseId, categoryId, lectureIds));

            // TF
            allSelectedQuestions.AddRange(await _questionRepository.GetAutoQuestionsFilteredAsync(
                "True/False", "Easy", requestDto.TfEasy, professorId, courseId, categoryId, lectureIds));

            allSelectedQuestions.AddRange(await _questionRepository.GetAutoQuestionsFilteredAsync(
                "True/False", "Medium", requestDto.TfMedium, professorId, courseId, categoryId, lectureIds));

            allSelectedQuestions.AddRange(await _questionRepository.GetAutoQuestionsFilteredAsync(
                "True/False", "Hard", requestDto.TfHard, professorId, courseId, categoryId, lectureIds));

            // Map to DTO
            var mappedQuestions = _mapper.Map<List<QuestionGetDto>>(allSelectedQuestions);

            return new QuestionFilterDto
            {
                Questions = mappedQuestions
            };

        }

        public async Task SaveGeneratedQuestionsAsync(ParsedQuestionsDto dto, int professorId)
        {
            foreach (var group in dto.QuestionGroups)
            {
                // Parse combined type string like "mcqEasy" or "truefalseMedium"
                var (typeName, difficultyName) = ParseTypeAndDifficulty(group.Type);

                typeName = typeName.Trim().ToLower(); // Normalize for consistency

                int questionTypeId = GetQuestionTypeIdByName(typeName);
                int difficultyLevelId = GetDifficultyLevelIdByName(difficultyName);

                foreach (var aiQuestion in group.Questions)
                {
                    var questionDto = new QuestionAddDto
                    {
                        Text = CleanQuestionText(aiQuestion.QuestionText),
                        QuestionTypeId = questionTypeId,
                        DifficultyLevelId = difficultyLevelId,
                        professorId = professorId,
                        CourseId = dto.CourseId,
                        LectureName = aiQuestion.LectureName,

                        Answers = (typeName == "tf" || typeName == "truefalse")
                            ? new List<AnswerAddDto>
                            {
                        new AnswerAddDto
                        {
                            Text = "True",
                            IsCorrect = aiQuestion.Answer.Trim()
                                .Equals("True", StringComparison.OrdinalIgnoreCase)
                        },
                        new AnswerAddDto
                        {
                            Text = "False",
                            IsCorrect = aiQuestion.Answer.Trim()
                                .Equals("False", StringComparison.OrdinalIgnoreCase)
                        }
                            }
                            : aiQuestion.Choices?.Select((choice, index) => new AnswerAddDto
                            {
                                Text = CleanAnswerChoice(choice),
                                IsCorrect = GetAnswerLetter(index) == aiQuestion.Answer.Trim().ToUpper()
                            }).ToList() ?? new List<AnswerAddDto>()
                    };

                    await AddQuestionAsync(questionDto);
                }
            }
        }


        private (string type, string difficulty) ParseTypeAndDifficulty(string combined)
    {
        combined = combined.ToLower();

        string[] types = { "mcq", "multiplechoice", "tf" };
        string[] difficulties = { "easy", "medium", "hard" };

        string type = types.FirstOrDefault(t => combined.Contains(t)) ?? "unknown";
        string difficulty = difficulties.FirstOrDefault(d => combined.Contains(d)) ?? "easy";

        return (type, difficulty);
    }

    private int GetQuestionTypeIdByName(string typeName)
    {
        return typeName.ToLower() switch
        {
            "mcq" or "multiplechoice" => 1,
            "tf" => 2,
            _ => 0
        };
    }

    private int GetDifficultyLevelIdByName(string difficulty)
    {
        return difficulty.ToLower() switch
        {
            "easy" => 1,
            "medium" => 2,
            "hard" => 3,
            _ => 1
        };
    }

    private string GetAnswerLetter(int index)
    {
        return ((char)('A' + index)).ToString(); // A, B, C...
    }

    private string CleanQuestionText(string text)
    {
        // Removes "Q1)", "Q2." or similar from the start
        return Regex.Replace(text, @"^Q\d*\)?\.?\s*", "", RegexOptions.IgnoreCase).Trim();
    }

    private string CleanAnswerChoice(string choice)
    {
        // Removes "A)", "B.", "C:" or similar from the start
        return Regex.Replace(choice, @"^[A-Da-d]\)?\.?\s*", "", RegexOptions.IgnoreCase).Trim();
    }





}

}

//public async Task AddQuestionAsync(QuestionAddDto questionDto)
//{
//    // Check if Lecture exists
//    var lecture = await _lectureRepository.GetByIdAsync(questionDto.LectureId);
//    if (lecture == null)
//    {
//        // Dynamically create lecture
//        lecture = new Lecture
//        {
//            Id = questionDto.LectureId,
//            LectureName = $"Lecture {questionDto.LectureId}",
//            CourseId = questionDto.CourseId // Ensure the relationship is valid
//        };
//        await _lectureRepository.AddAsync(lecture);
//    }

//    // Map question DTO to entity
//    var question = _mapper.Map<Question>(questionDto);

//    // Add question to repository
//    await _questionRepository.AddAsync(question);
//}

