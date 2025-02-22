using AutoMapper;
using BusinessLogicLayer.Dtos.AnswerDto;
using BusinessLogicLayer.Dtos.QuestionDto;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

public class CsvImportService : ICsvImportService
{
    private readonly ICategoryService _categoryService;
    private readonly ICourseService _courseService;
    private readonly IQuestionService _questionService;
    private readonly IMapper _mapper;

    public CsvImportService(ICategoryService categoryService, ICourseService courseService, IQuestionService questionService, IMapper mapper)
    {
        _categoryService = categoryService;
        _courseService = courseService;
        _questionService = questionService;
        _mapper = mapper;
    }

    public async Task ImportQuestionsAsync(Stream importStream, string fileName, int professorId)
    {
        using (var stream = new StreamReader(importStream))
        {
            var csvReader = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture));
            var questions = csvReader.GetRecords<QuestionImportDto>().ToList();

            foreach (var question in questions)
            {
                var category = await _categoryService.GetCategoryByNameAndProfessorAsync(question.Category, professorId);
                var course = await _courseService.GetCourseByNameAndProfessorAsync(question.Course, professorId);

                // Ensure category and course exist
                if (category == null)
                {
                    category = await _categoryService.AddCategoryAsync(question.Category, professorId);
                }
                if (course == null)
                {
                    course = await _courseService.AddCourseAsync(question.Course, category.Id, professorId);
                }

                var model = new QuestionAddDto
                {
                    Text = question.QuestionText,
                    CategoryId = category.Id,
                    CourseId = course.Id,
                    DifficultyLevelId = GetDifficultyLevelId(question.DifficultyLevel),
                    QuestionTypeId = GetQuestionTypeId(question.QuestionType),
                    Answers = new List<AnswerAddDto>
                    {
                        new AnswerAddDto { Text = question.Answer1, IsCorrect = question.CorrectAnswerIndex == 0 },
                        new AnswerAddDto { Text = question.Answer2, IsCorrect = question.CorrectAnswerIndex == 1 },
                        new AnswerAddDto { Text = question.Answer3, IsCorrect = question.CorrectAnswerIndex == 2 },
                        new AnswerAddDto { Text = question.Answer4, IsCorrect = question.CorrectAnswerIndex == 3 }
                    },
                    professorId = professorId
                };

                await _questionService.AddQuestionAsync(model);
            }
        }
    }

    private int GetDifficultyLevelId(string difficultyLevel)
    {
        return difficultyLevel.ToLower() switch
        {
            "easy" => 1,
            "medium" => 2,
            "hard" => 3,
            _ => 1, // Default to Easy if not recognized
        };
    }

    private int GetQuestionTypeId(string questionType)
    {
        return questionType.ToLower() switch
        {
            "multiplechoice" => 1,
            "truefalse" => 2,
            _ => 1, // Default to Multiple Choice if not recognized
        };
    }
}
