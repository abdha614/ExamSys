//using AutoMapper;
//using BusinessLogicLayer.Dtos.AnswerDto;
//using BusinessLogicLayer.Dtos.QuestionDto;
//using BusinessLogicLayer.Interfaces;
//using CsvHelper.Configuration;
//using CsvHelper;
//using System.Globalization;

//public class CsvImportService : IQuestionImportService
//{
//    private readonly ICategoryService _categoryService;
//    private readonly ICourseService _courseService;
//    private readonly IQuestionService _questionService;
//    private readonly IMapper _mapper;

//    public CsvImportService(ICategoryService categoryService, ICourseService courseService, IQuestionService questionService, IMapper mapper)
//    {
//        _categoryService = categoryService;
//        _courseService = courseService;
//        _questionService = questionService;
//        _mapper = mapper;
//    }

//    public bool CanHandle(string fileExtension) => fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase);

//    public async Task ImportQuestionsAsync(Stream importStream, string fileName, int professorId)
//    {
//        using (var stream = new StreamReader(importStream))
//        using (var csvReader = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture)))
//        {
//            var records = csvReader.GetRecords<dynamic>().ToList();

//            foreach (var record in records)
//            {
//                var answers = new List<string>();
//                string correctAnswerText = string.Empty;

//                var dict = (IDictionary<string, object>)record;
//                foreach (var key in dict.Keys)
//                {
//                    if (key.StartsWith("Answer")) // Detect answer columns
//                    {
//                        string answer = dict[key]?.ToString();
//                        if (!string.IsNullOrWhiteSpace(answer))
//                        {
//                            answers.Add(answer);
//                        }
//                    }
//                    else if (key == "CorrectAnswer") // Use the text of the correct answer
//                    {
//                        correctAnswerText = dict[key]?.ToString();
//                    }
//                }

//                var questionText = dict["QuestionText"].ToString();

//                Check if the question already exists in the database
//                var doesQuestionExist = await _questionService.DoesQuestionExistAsync(questionText, professorId);
//                if (doesQuestionExist)
//                {
//                    continue; // Skip duplicate questions
//                }

//                var category = await _categoryService.GetCategoryByNameAndProfessorAsync(dict["Category"].ToString(), professorId)
//                              ?? await _categoryService.AddCategoryAsync(dict["Category"].ToString(), professorId);

//                var course = await _courseService.GetCourseByNameAndProfessorAsync(dict["Course"].ToString(), professorId)
//                           ?? await _courseService.AddCourseAsync(dict["Course"].ToString(), category.Id, professorId);

//                var model = new QuestionAddDto
//                {
//                    Text = questionText,
//                    CategoryId = category.Id,
//                    CourseId = course.Id,
//                    DifficultyLevelId = GetDifficultyLevelId(dict["DifficultyLevel"].ToString()),
//                    QuestionTypeId = GetQuestionTypeId(dict["QuestionType"].ToString()),
//                    Answers = answers.Select(answer => new AnswerAddDto
//                    {
//                        Text = answer,
//                        IsCorrect = string.Equals(answer.Trim(), correctAnswerText.Trim(), StringComparison.OrdinalIgnoreCase)
//                    }).ToList(),
//                    professorId = professorId
//                };

//                await _questionService.AddQuestionAsync(model);
//            }
//        }
//    }

//    private int GetDifficultyLevelId(string difficultyLevel) => difficultyLevel.ToLower() switch
//    {
//        "easy" => 1,
//        "medium" => 2,
//        "hard" => 3,
//        _ => 1,
//    };

//    private int GetQuestionTypeId(string questionType) => questionType.ToLower() switch
//    {
//        "multiplechoice" => 1,
//        "truefalse" => 2,
//        _ => 1,
//    };
//}




/////////////////////
using AutoMapper;
using BusinessLogicLayer.Dtos.AnswerDto;
using BusinessLogicLayer.Dtos.QuestionDto;
using BusinessLogicLayer.Interfaces;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using BusinessLogicLayer.Services;
using System.Text;
using DataAccessLayer.Models;

public class CsvImportService : IQuestionImportService
{
    private readonly ICategoryService _categoryService;
    private readonly ICourseService _courseService;
    private readonly IQuestionService _questionService;
    private readonly IMapper _mapper;
    private readonly IQuestionTypeService _questionTypeService;
    private readonly IDifficultyLevelService _difficultyLevelService;

    public CsvImportService(ICategoryService categoryService, ICourseService courseService, IQuestionService questionService, IQuestionTypeService questionTypeService, IDifficultyLevelService difficultyLevelService, IMapper mapper)
    {
        _categoryService = categoryService;
        _courseService = courseService;
        _questionService = questionService;
        _questionTypeService = questionTypeService;
        _mapper = mapper;
        _difficultyLevelService = difficultyLevelService;
    }

    public bool CanHandle(string fileExtension) => fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase);

    public async Task ImportQuestionsAsync(Stream importStream, string fileName, int professorId)
    {
        using (var stream = new StreamReader(importStream))
        using (var csvReader = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            var records = csvReader.GetRecords<dynamic>().ToList();

            foreach (var record in records)
            {
                var answers = new List<string>();
                string correctAnswerText = string.Empty;

                var dict = (IDictionary<string, object>)record;
                foreach (var key in dict.Keys)
                {
                    if (key.StartsWith("Answer")) // Detect answer columns
                    {
                        string answer = dict[key]?.ToString();
                        if (!string.IsNullOrWhiteSpace(answer))
                        {
                            answers.Add(answer);
                        }
                    }
                    else if (key == "CorrectAnswer") // Use the text of the correct answer
                    {
                        correctAnswerText = dict[key]?.ToString();
                    }
                }

                var questionText = dict["QuestionText"].ToString();

                // Check if the question already exists in the database
                var doesQuestionExist = await _questionService.DoesQuestionExistAsync(questionText, professorId);
                if (doesQuestionExist)
                {
                    continue; // Skip duplicate questions
                }

                // Get existing category - skip if not found
                //var categoryId = await _categoryService.GetCategoryIdByNameAndProfessorAsync(dict["Category"].ToString(), professorId);
                //if (categoryId == null)
                //{
                //    continue; // Skip if category doesn't exist
                //}

                // Get existing course - skip if not found
                var courseId = await _courseService.GetCourseIdByNameAndProfessorAsync(dict["Course"].ToString(), professorId);
                if (courseId == null )
                {
                    continue; // Skip if course doesn't exist or doesn't belong to the category
                }
                var questionTypeId = await _questionTypeService.GetQuestionTypeByNameAsync(dict["QuestionType"].ToString());

                var difficultyLevelId = await _difficultyLevelService.GetDifficultyLevelByNameAsync(dict["DifficultyLevel"]?.ToString());

                var lectureName = dict["Lecture"]?.ToString();
                if (string.IsNullOrWhiteSpace(lectureName))
                {
                    continue; // Skip if Lecture column is empty or null
                }

                var model = new QuestionAddDto
                {
                    Text = questionText,
                  //  CategoryId = categoryId.Value,
                    CourseId = courseId.Value,
                    DifficultyLevelId = difficultyLevelId, 
                    QuestionTypeId = questionTypeId,
                    Answers = answers.Select(answer => new AnswerAddDto
                    {
                        Text = answer,
                        IsCorrect = string.Equals(answer.Trim(), correctAnswerText.Trim(), StringComparison.OrdinalIgnoreCase)
                    }).ToList(),
                    professorId = professorId,
                    LectureName = lectureName // Add LectureName here
                };

                await _questionService.AddQuestionAsync(model);
            }
        }

    }
    public string GenerateTemplate(string templateType)
    {
        // Define the headers
        var headers = new List<string>
        {
            "QuestionText", "Course","Lecture","QuestionType", "DifficultyLevel",
            "Answer1", "Answer2", "Answer3", "Answer4", "CorrectAnswer"
        };

        // Define example rows for Simple Template
        var sampleData = new List<string[]>
        {
            new[] { "What is the capital of France?", "Europe Studies", "Lecture 1", "Multiple Choice", "Easy", "Paris", "London", "Berlin", "Paris" },
            new[] { "What is 2 + 2?", "Algebra Basics", "Lecture 1", "Multiple Choice", "Easy", "3", "4", "5", "4" }
        };

        // Generate CSV based on template type
        var csvContent = new StringBuilder();
        csvContent.AppendLine(string.Join(",", headers));
        if (templateType == "Simple")
        {
            foreach (var row in sampleData)
            {
                csvContent.AppendLine(string.Join(",", row));
            }
        }
        return csvContent.ToString();

    }
}