//using AutoMapper;
//using BusinessLogicLayer.Dtos.AnswerDto;
//using BusinessLogicLayer.Dtos.QuestionDto;
//using BusinessLogicLayer.Interfaces;
//using DocumentFormat.OpenXml.Packaging;
//using System.Text.RegularExpressions;

//public class DocxImportService : IQuestionImportService
//{
//    private readonly ICategoryService _categoryService;
//    private readonly ICourseService _courseService;
//    private readonly IQuestionService _questionService;
//    private readonly IMapper _mapper;

//    public DocxImportService(ICategoryService categoryService, ICourseService courseService, IQuestionService questionService, IMapper mapper)
//    {
//        _categoryService = categoryService;
//        _courseService = courseService;
//        _questionService = questionService;
//        _mapper = mapper;
//    }

//    public bool CanHandle(string fileExtension) => fileExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase);

//    public async Task ImportQuestionsAsync(Stream importStream, string fileName, int professorId)
//    {
//        using var memoryStream = new MemoryStream();
//        await importStream.CopyToAsync(memoryStream);
//        memoryStream.Position = 0;

//        using var wordDoc = WordprocessingDocument.Open(memoryStream, false);
//        var paragraphs = wordDoc.MainDocumentPart.Document.Body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
//        var text = string.Join("\n", paragraphs.Select(p => p.InnerText));

//        // Debugging: Print extracted text
//        Console.WriteLine($"Extracted Text: {text}");

//        var records = ParseQuestionsFromText(text);

//        foreach (var record in records)
//        {
//            if (!record.TryGetValue("QuestionText", out string questionText))
//            {
//                Console.WriteLine("Warning: Skipping entry due to missing 'QuestionText'.");
//                continue;
//            }

//            // Check if the question already exists in the database
//            var doesQuestionExist = await _questionService.DoesQuestionExistAsync(questionText, professorId);
//            if (doesQuestionExist)
//            {
//                continue; // Skip duplicate questions
//            }


//            var categoryName = record.GetValueOrDefault("Category", "General");
//            //var category = await _categoryService.GetCategoryByNameAndProfessorAsync(categoryName, professorId)
//            //              ?? await _categoryService.AddCategoryAsync(categoryName, professorId);

//            var courseName = record.GetValueOrDefault("Course", "Uncategorized");
//            //var course = await _courseService.GetCourseByNameAndProfessorAsync(courseName, professorId)
//            //           ?? await _courseService.AddCourseAsync(courseName, category.Id, professorId);

//            var answers = new List<string>();
//            string correctAnswerText = record.GetValueOrDefault("Correct Answer", "");

//            foreach (var key in record.Keys)
//            {
//                if (key.StartsWith("Answer")) // Detect answer columns
//                {
//                    string answer = record[key];
//                    if (!string.IsNullOrWhiteSpace(answer))
//                    {
//                        answers.Add(answer);
//                    }
//                }
//            }

//            var model = new QuestionAddDto
//            {
//                Text = questionText,
//                //CategoryId = category.Id,
//                //CourseId = course.Id,
//                DifficultyLevelId = GetDifficultyLevelId(record.GetValueOrDefault("Difficulty", "Easy")),
//                QuestionTypeId = GetQuestionTypeId(record.GetValueOrDefault("QuestionType", "MultipleChoice")),
//                Answers = answers.Select(answer => new AnswerAddDto
//                {
//                    Text = answer,
//                    IsCorrect = string.Equals(answer.Trim(), correctAnswerText.Trim(), StringComparison.OrdinalIgnoreCase)
//                }).ToList(),
//                professorId = professorId
//            };

//            await _questionService.AddQuestionAsync(model);
//        }
//    }

//    private List<Dictionary<string, string>> ParseQuestionsFromText(string text)
//    {
//        var questions = new List<Dictionary<string, string>>();
//        var questionBlocks = Regex.Split(text, @"\n---\n"); // Splitting questions by "---"

//        foreach (var block in questionBlocks)
//        {
//            var lines = block.Split('\n').Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToList();
//            if (!lines.Any(l => l.StartsWith("Question: ")))
//            {
//                Console.WriteLine("Warning: Skipping invalid question block.");
//                continue;
//            }

//            var record = new Dictionary<string, string>();

//            foreach (var line in lines)
//            {
//                var parts = line.Split(':', 2);
//                if (parts.Length == 2)
//                {
//                    record[parts[0].Trim()] = parts[1].Trim();
//                }
//            }

//            // Ensure key consistency
//            if (record.ContainsKey("Question"))
//            {
//                record["QuestionText"] = record["Question"]; // Rename for compatibility
//                record.Remove("Question");
//            }

//            if (record.ContainsKey("Type"))
//            {
//                record["QuestionType"] = record["Type"]; // Rename for compatibility
//                record.Remove("Type");
//            }

//            questions.Add(record);
//        }

//        return questions;
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
using BusinessLogicLayer.Dtos.AnswerDto;
using BusinessLogicLayer.Dtos.QuestionDto;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

public class DocxImportService : IQuestionImportService
{
    private readonly ICategoryService _categoryService;
    private readonly ICourseService _courseService;
    private readonly IQuestionService _questionService;
    private readonly IQuestionTypeService _questionTypeService;
    private readonly IDifficultyLevelService _difficultyLevelService;

    public DocxImportService(
        ICategoryService categoryService,
        ICourseService courseService,
        IQuestionService questionService,
        IQuestionTypeService questionTypeService,
        IDifficultyLevelService difficultyLevelService)
    {
        _categoryService = categoryService;
        _courseService = courseService;
        _questionService = questionService;
        _questionTypeService = questionTypeService;
        _difficultyLevelService = difficultyLevelService;
    }

    public bool CanHandle(string fileExtension) =>
        fileExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase);

    public async Task ImportQuestionsAsync(Stream importStream, string fileName, int professorId)
    {
        using var wordDoc = WordprocessingDocument.Open(importStream, false);
        var body = wordDoc.MainDocumentPart.Document.Body;
        var paragraphs = body.Elements<Paragraph>().ToList();

        QuestionAddDto currentQuestion = null;
        var questions = new List<QuestionAddDto>();

        foreach (var para in paragraphs)
        {
            string text = para.InnerText.Trim();

            if (string.IsNullOrWhiteSpace(text)) continue;

            if (text.StartsWith("Question:"))
            {
                if (currentQuestion != null)
                    questions.Add(currentQuestion);

                currentQuestion = new QuestionAddDto
                {
                    Text = text.Substring(9).Trim(),
                    Answers = new List<AnswerAddDto>(),
                    professorId = professorId
                };
            }
            //else if (text.StartsWith("Category:") && currentQuestion != null)
            //{
            //    var categoryId = await _categoryService.GetCategoryIdByNameAndProfessorAsync(text.Substring(9).Trim(), professorId);
            //    if (categoryId == null) continue;
            //    currentQuestion.CategoryId = categoryId.Value;
            //}
            else if (text.StartsWith("Course:") && currentQuestion != null)
            {
                var courseId = await _courseService.GetCourseIdByNameAndProfessorAsync(text.Substring(7).Trim(), professorId);
                if (courseId == null) continue;
                currentQuestion.CourseId = courseId.Value;
            }
            else if (text.StartsWith("Lecture:") && currentQuestion != null)
            {
                currentQuestion.LectureName = text.Substring(8).Trim();
            }
            else if (text.StartsWith("QuestionType:") && currentQuestion != null)
            {
                var qTypeId = await _questionTypeService.GetQuestionTypeByNameAsync(text.Substring(13).Trim());
                currentQuestion.QuestionTypeId = qTypeId;
            }
            else if (text.StartsWith("Difficulty:") && currentQuestion != null)
            {
                var diffText = text.Split(':', 2).ElementAtOrDefault(1)?.Trim();
                if (!string.IsNullOrWhiteSpace(diffText))
                {
                    var diffId = await _difficultyLevelService.GetDifficultyLevelByNameAsync(diffText);
                    currentQuestion.DifficultyLevelId = diffId;
                }

            }
            else if (text.StartsWith("Answer:") && currentQuestion != null)
            {
                var answerText = text.Substring(7).Trim();
                currentQuestion.Answers.Add(new AnswerAddDto { Text = answerText, IsCorrect = false });
            }
            else if (text.StartsWith("Correct:") && currentQuestion != null)
            {
                var correctAnswer = text.Substring(8).Trim();
                foreach (var answer in currentQuestion.Answers)
                {
                    answer.IsCorrect = answer.Text.Trim().Equals(correctAnswer, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        if (currentQuestion != null)
            questions.Add(currentQuestion);

        foreach (var question in questions)
        {
            if (!string.IsNullOrWhiteSpace(question.Text))
            {
                bool exists = await _questionService.DoesQuestionExistAsync(question.Text, professorId);
                if (!exists)
                    await _questionService.AddQuestionAsync(question);
            }
        }
    }

    public string GenerateTemplate(string templateType)
    {
        var sb = new StringBuilder();

        if (templateType.Equals("Blank", StringComparison.OrdinalIgnoreCase))
        {
            sb.AppendLine("Question: ");
           // sb.AppendLine("Category: ");
            sb.AppendLine("Course: ");
            sb.AppendLine("Lecture: ");
            sb.AppendLine("QuestionType: ");
            sb.AppendLine("Difficulty: ");
            sb.AppendLine("Answer: ");
            sb.AppendLine("Answer: ");
            sb.AppendLine("Answer: ");
            sb.AppendLine("Correct: ");
        }
        else // Simple
        {
            sb.AppendLine("Question: What is the capital of France?");
          //  sb.AppendLine("Category: Geography");
            sb.AppendLine("Course: Europe Studies");
            sb.AppendLine("Lecture: Lecture 1");
            sb.AppendLine("QuestionType: Multiple Choice");
            sb.AppendLine("Difficulty: Easy");
            sb.AppendLine("Answer: Paris");
            sb.AppendLine("Answer: London");
            sb.AppendLine("Answer: Berlin");
            sb.AppendLine("Correct: Paris");
            sb.AppendLine("---------------");

            sb.AppendLine("Question: What is 2 + 2?");
          //  sb.AppendLine("Category: Math");
            sb.AppendLine("Course: Algebra Basics");
            sb.AppendLine("Lecture: Lecture 1");
            sb.AppendLine("QuestionType: Multiple Choice");
            sb.AppendLine("Difficulty: Easy");
            sb.AppendLine("Answer: 3");
            sb.AppendLine("Answer: 4");
            sb.AppendLine("Answer: 5");
            sb.AppendLine("Correct: 4");
        }

        return sb.ToString();
    }
}

