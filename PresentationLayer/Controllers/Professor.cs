using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.QuestionDto;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using PresentationLayer.Extensions;
using PresentationLayer.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using BusinessLogicLayer.Dtos.CategoryDto;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Dtos.CourseDto;
using System.Text;
using PresentationLayer.ViewModels.ExamViewModel;
using BusinessLogicLayer.Dtos.AnswerDto;
using Rotativa;
using Rotativa.AspNetCore;
using System;
using BusinessLogicLayer.Dtos.ExamDto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using BusinessLogicLayer.Questions_Mangment.Interfaces;
using Rotativa.AspNetCore.Options;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using BusinessLogicLayer.Questions_Mangment.Services;
using Newtonsoft.Json;
using DataAccessLayer.Models;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using BusinessLogicLayer.Dtos.LectureDto;
using static PresentationLayer.ViewModels.LectureOptionViewModel;
using BusinessLogicLayer.RAG.Interfaces;





[Authorize(Policy = "ProfessorPolicy")]
public class ProfessorController : Controller
{
    private readonly IQuestionService _questionService;
    private readonly ICategoryService _categoryService;
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;
    private readonly IQuestionTypeService _questionTypeService;
    private readonly IEnumerable<IQuestionImportService> _importServices;
    private readonly IExamService _examService ;
    
    private readonly IWebHostEnvironment _env;
    private readonly IQuestionParser _questionParser;
    private readonly ILectureFileService _lectureFileService;
    private readonly IRagService _ragService;





    public ProfessorController(IQuestionService questionService,IExamService examService, IRagService ragService, ILectureFileService lectureFileService, IQuestionParser questionParser, IWebHostEnvironment env, ICategoryService categoryService, ICourseService courseService, IQuestionTypeService questionTypeService, IMapper mapper, IEnumerable<IQuestionImportService> importServices)
    {
        _questionService = questionService;
        _categoryService = categoryService;
        _lectureFileService = lectureFileService;
        _courseService = courseService;
        _questionTypeService = questionTypeService;
        _mapper = mapper;
        _importServices = importServices;
        _examService = examService;
        
        _env = env;
        _questionParser = questionParser;
        _ragService = ragService;
    }
    public async Task<IActionResult> ManageQuestions(
    int? questionTypeId = null,
    int? difficultyLevelId = null,
    int? categoryId = null,
    int? courseId = null,
    int? lectureId = null)
    {
        var professorId = User.GetProfessorId(); // Get the professor's ID from claims

        //  Fetch filtered questions and filter options
        var filterData = await _questionService.GetQuestionsFilteredAsync(professorId, questionTypeId, difficultyLevelId, categoryId, courseId,lectureId);
        var questionViewModels = _mapper.Map<List<QuestionViewModel>>(filterData.Questions);

        //  Map to ViewModel
        var viewModel = new ManageQuestionsViewModel
        {
            Questions = _mapper.Map<List<QuestionViewModel>>(filterData.Questions),

            // Populate dropdowns
            Categories = filterData.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == categoryId
            }).ToList(),

            Courses = filterData.Courses.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == courseId
            }).ToList(),


            QuestionTypes = filterData.QuestionTypes.Select(qt => new SelectListItem
            {
                Value = qt.Id.ToString(),
                Text = qt.Type,
                Selected = qt.Id == questionTypeId
            }).ToList(),

            DifficultyLevels = filterData.DifficultyLevels.Select(dl => new SelectListItem
            {
                Value = dl.Id.ToString(),
                Text = dl.Name,
                Selected = dl.Id == difficultyLevelId
            }).ToList(),

            SelectedCourseId = courseId,
            SelectedLectureId = lectureId,
        };

        // Check if filters are applied and no results found
        bool isFilterApplied = questionTypeId.HasValue || difficultyLevelId.HasValue || categoryId.HasValue || courseId.HasValue;
        //if (isFilterApplied && !questionViewModels.Any())
        //{
        //    ViewBag.ErrorMessage = "No questions found for the selected filters.";
        //}

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> FilterQuestions(
    int? questionTypeId,
    int? difficultyLevelId,
    int? categoryId,
    int? courseId,
    int? lectureId)
    {
        var professorId = User.GetProfessorId();

        var filterData = await _questionService.GetQuestionsFilteredAsync(
            professorId, questionTypeId, difficultyLevelId, categoryId, courseId, lectureId);

        var mappedQuestions = _mapper.Map<List<QuestionViewModel>>(filterData.Questions);

        return PartialView("_FilteredQuestionsPartial1", mappedQuestions); // Your partial view
    }



    public IActionResult ChooseQuestionType()
    {
        return View();
    }

    public async Task<IActionResult> AddMultipleChoiceQuestion()
    {
        var professorId = User.GetProfessorId();

        // Fetch all related data
        var professorData = await _categoryService.GetAllRelatedDataForProfessorAsync(professorId);

        // Initialize the model
        var model = new QuestionAddViewModel
        {
            QuestionTypeId = 1, // Set the default QuestionTypeId for multiple-choice question
            Answers = new List<AnswerAddViewModel>
        {
            new AnswerAddViewModel(),
            new AnswerAddViewModel(),
            new AnswerAddViewModel()
        },
            Courses = professorData.Courses,
          //  Categories = professorData.Categories,
            DifficultyLevels = professorData.DifficultyLevels,
            
        };

        return View(model);
    }



    [HttpPost]
    public async Task<IActionResult> AddMultipleChoiceQuestion(QuestionAddViewModel model)
    {
        var professorId = User.GetProfessorId();
        model.professorId = professorId; // Ensure the UserId is set to the current professor's ID

        if (ModelState.IsValid)
        {

            // Set the correct answer based on CorrectAnswerIndex
            for (int i = 0; i < model.Answers.Count; i++)
            {
                model.Answers[i].IsCorrect = (i == model.CorrectAnswerIndex);
            }

            // Mapping the view model to DTO
            var questionDto = _mapper.Map<QuestionAddDto>(model);

            // Calling the service to add the question
            await _questionService.AddQuestionAsync(questionDto);

            TempData["SuccessMessage"] = " question add successfully.";

            // Redirect to ManageQuestions after successful addition
            return RedirectToAction("ManageQuestions");
        }

        // If the form is not valid, reload necessary data and return the same view
      //  var professorData = await _categoryService.GetAllRelatedDataForProfessorAsync(professorId);

      //  // Reload categories, courses, lectures, etc.
      //  model.Categories = professorData.Categories;
      ////  model.Courses = professorData.Categories.SelectMany(c => c.Courses).ToList();
      //  model.DifficultyLevels = professorData.DifficultyLevels;

      //  TempData["ErrorMessage"] = "There were errors with your submission. Please fix them and try again.";

        // Return the same view with the updated model
        return View(model);
    }

    public async Task<IActionResult> AddTrueFalseQuestion()
    {
        var professorId = User.GetProfessorId();

        // Fetch all related data
        var professorData = await _categoryService.GetAllRelatedDataForProfessorAsync(professorId);

        // Initialize the model
        var model = new QuestionAddViewModel
        {
            QuestionTypeId = 2, // Set QuestionTypeId for True/False question
            Answers = new List<AnswerAddViewModel>
        {
            new AnswerAddViewModel { Text = "True" },
            new AnswerAddViewModel { Text = "False" }
        },
            Courses = professorData.Courses,
            DifficultyLevels = professorData.DifficultyLevels
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddTrueFalseQuestion(QuestionAddViewModel model)
    {
        var professorId = User.GetProfessorId();
        model.professorId = professorId;

        if (ModelState.IsValid)
        {
            // Set the correct answer based on CorrectAnswerIndex
            for (int i = 0; i < model.Answers.Count; i++)
            {
                model.Answers[i].IsCorrect = (i == model.CorrectAnswerIndex);
            }

            var questionDto = _mapper.Map<QuestionAddDto>(model);
            await _questionService.AddQuestionAsync(questionDto);

            TempData["SuccessMessage"] = "Question added successfully.";
            return RedirectToAction("ManageQuestions");
        }

        // Reload data if validation fails
        var professorData = await _categoryService.GetAllRelatedDataForProfessorAsync(professorId);
        model.Categories = professorData.Categories;
        model.DifficultyLevels = professorData.DifficultyLevels;

        TempData["ErrorMessage"] = "There were errors with your submission. Please fix them and try again.";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetCoursesByCategory(int categoryId)
    {
        var professorId = User.GetProfessorId();

        var professorData = await _categoryService.GetAllRelatedDataForProfessorAsync(professorId);
        var category = professorData.Categories.FirstOrDefault(c => c.Id == categoryId);

        if (category == null)
        {
            return Json(new List<object>());
        }

        var courses = category.Courses
            .Select(course => new
            {
                course.Id,
                course.Name
            });

        return Json(courses);
    }
    [HttpGet]
    public async Task<IActionResult> GetCoursesByyCategory(int categoryId)
    {
        var courses = await _categoryService.GetCoursesWithQuestionsByCategoryAsync(categoryId);

        if (courses == null || !courses.Any())
        {
            return Json(new List<object>());
        }

        var courseDtos = courses.Select(course => new
        {
            course.Id,
            course.Name
        });

        return Json(courseDtos);
    }
    [HttpGet]
    //[HttpGet("Professor/GetLecturesByCourse/{courseId}")]
    public async Task<IActionResult> GetLecturesByCourse(int courseId)
    {
        var lectures = await _courseService.GetLecturesByCourseAsync(courseId);

        if (lectures == null || !lectures.Any())
        {
            return Json(new List<object>()); // Return empty JSON if no lectures are found
        }

        var lectureDtos = lectures.Select(lecture => new
        {
            lecture.Id,
            lecture.LectureName
        });

        return Json(lectureDtos); // Return lectures in JSON format
    }

    //// Action to add a new category with professor ID
    //[HttpPost]
    //public async Task<IActionResult> AddCategory(string name)
    //{
    //    var professorId = User.GetProfessorId();


    //    // Check if the category already exists for this professor
    //    var existingCategory = await _categoryService.GetCategoryByNameAndProfessorAsync(name, professorId);
    //    if (existingCategory != null)
    //    {
    //        return Json(new
    //        {
    //            error = true,
    //            errorMessage = "Category already exists for this professor."
    //        });
    //    }

    //    // Add the new category
    //    var category = await _categoryService.AddCategoryAsync(name, professorId);
    //    var categoryDto = _mapper.Map<CategoryGetDto>(category);

    //    return Json(categoryDto);
    //}
    public async Task<IActionResult> ManageCourses(int? selectedCategoryId)
    {
        var professorId = User.GetProfessorId();

        var categories = await _categoryService.GetCategoriesWithCoursesByProfessorIdAsync(professorId);

        var model = new ManageCoursesViewModel
        {
            Categories = categories,
            SelectedCategoryId = selectedCategoryId
        };

        return View(model);
    }
    //[HttpPost]
    //public IActionResult Delete(int id)
    //{
    //    var success = _courseService.(id);
    //    if (success)
    //    {
    //        TempData["SuccessMessage"] = "Course deleted successfully.";
    //    }
    //    else
    //    {
    //        TempData["ErrorMessage"] = "Failed to delete course.";
    //    }

    //    return RedirectToAction(nameof(ManageCourses));
    //}
    [HttpPost]
    public async Task<IActionResult> RemoveCourse( int courseId)
    {
        await _courseService.DeleteCourseWithDependenciesAsync(courseId);
        return RedirectToAction("ManageCourses");

    }

    // Action to add a new course with professor ID
    //[HttpPost]
    //public async Task<IActionResult> AddCourse(string name, int categoryId)
    //{
    //    var professorId = User.GetProfessorId();

    //    // Check for duplicates in the category for this professor
    //    var existingCourse = await _courseService.GetCourseByNameCategoryAndProfessorAsync(name, categoryId, professorId);
    //    if (existingCourse != null)
    //    {
    //        return Json(new
    //        {
    //            error = true,
    //            errorMessage = "Course already exists for this category."
    //        });
    //    }

    //    // Add the course if it does not already exist
    //    var course = await _courseService.AddCourseAsync(name, categoryId, professorId);

    //    return Json(new
    //    {
    //        error = false,
    //        data = course
    //    });
    //}
    [HttpGet]
    public async Task<IActionResult> AddCourse()
    {
        var professorId = User.GetProfessorId();
        // Fetch the list of categories for the dropdown, filtering by ProfessorId
        var categories = await _categoryService.GetCategoriesByProfessorAsync(professorId);
        var categoryList = categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
        }).ToList();

        // Create the view model
        var model = new AddCourseViewModel
        {
            Categories = categoryList,
            ProfessorId= professorId

        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddCourse(AddCourseViewModel model)
    {
        // Check if the course already exists
        var courseExists = await _courseService.GetCourseByNameCategoryAndProfessorAsync(
            model.CourseName, model.SelectedCategoryId, model.ProfessorId);

        if (courseExists != null)
        {
            TempData["ErrorMessage"] = "The course already exists.";
            return RedirectToAction("AddCourse");
        }

        // Use AutoMapper to map AddCourseViewModel to CourseAddDto
        var courseAddDto = _mapper.Map<CourseAddDto>(model);

        await _courseService.AddCourseAsync(courseAddDto);

        TempData["SuccessMessage"] = "Course added successfully!";
        return RedirectToAction("ManageCourses"); // Redirect to the course list page
    }



    [HttpGet]
    public async Task<IActionResult> EditQuestion(int id)
    {
        var professorId = User.GetProfessorId();

        var questionDto = await _questionService.GetQuestionByIdAsync(id);
        if (questionDto == null)
        {
            return NotFound();
        }

        var professorData = await _categoryService.GetAllRelatedDataForProfessorAsync(professorId);

        // Map to ViewModel
        var model = _mapper.Map<QuestionUpdateViewModel>(questionDto);

        // Set additional view data
        //model.Categories = professorData.Categories;
        model.DifficultyLevels = professorData.DifficultyLevels;
        model.Courses = professorData.Courses;

        // Find the index of the correct answer
        //   model.CorrectAnswerIndex = questionDto.Answers.FindIndex(a => a.IsCorrect);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditQuestion(QuestionUpdateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var professorId = User.GetProfessorId(); // Ensure the UserId is set to the current professor's ID
            model.professorId = professorId;

            // Set the correct answer based on CorrectAnswerIndex
            for (int i = 0; i < model.Answers.Count; i++)
            {
                model.Answers[i].IsCorrect = (i == model.CorrectAnswerIndex);
            }

            var questionDto = _mapper.Map<QuestionUpdateDto>(model);
            await _questionService.UpdateQuestionAsync( questionDto);

            return RedirectToAction("ManageQuestions");
        }

        //var categories = await _categoryService.GetCategoriesByProfessorAsync(model.professorId);
        //var courses = await _courseService.GetCoursesByProfessorAsync(model.professorId);

        //ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
        //ViewBag.Courses = new SelectList(courses, "Id", "Name", model.CourseId);

        return View(model);
    }

    // Action to delete a question
    [HttpPost]

    public async Task<IActionResult> DeleteQuestion(int id)
    {
        await _questionService.DeleteQuestionAsync(id);
        return RedirectToAction("ManageQuestions");
    }

    [HttpPost]
    public async Task<IActionResult> ImportQuestions(IFormFile importFile)
    {
        var professorId = User.GetProfessorId();

        if (importFile == null || importFile.Length == 0)
        {
            TempData["ErrorMessage"] = "The uploaded file is empty.";
            return RedirectToAction("ChooseQuestionType");
        }

        var fileExtension = Path.GetExtension(importFile.FileName).ToLower();

        var importService = _importServices.FirstOrDefault(s => s.CanHandle(fileExtension));

        if (importService == null)
        {
            TempData["ErrorMessage"] = "Unsupported file format. Please upload a CSV or DOCX file.";
            return RedirectToAction("ChooseQuestionType");
        }

        try
        {
            using var stream = importFile.OpenReadStream();
            await importService.ImportQuestionsAsync(stream, importFile.FileName, professorId);
            TempData["SuccessMessage"] = "Questions imported successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error importing file: {ex.Message}";
            return RedirectToAction("ChooseQuestionType");

        }

        return RedirectToAction("ManageQuestions");
    }

    // Endpoint to Download CSV Template
    //[HttpGet]
    //public IActionResult DownloadTemplate(string templateType, string fileExtension)
    //{
    //    // Identify the appropriate import service based on file extension
    //    var importService = _importServices.FirstOrDefault(service => service.CanHandle(fileExtension));

    //    if (importService == null)
    //    {
    //        return BadRequest("Unsupported file format.");
    //    }

    //    // Generate the template using the appropriate service
    //    var template = importService.GenerateTemplate(templateType);

    //    if (string.IsNullOrWhiteSpace(template))
    //    {
    //        return StatusCode(500, "Failed to generate template.");
    //    }

    //    // Determine the MIME type dynamically
    //    var mimeType = fileExtension switch
    //    {
    //        ".csv" => "text/csv",
    //        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    //        _ => "application/octet-stream" // Fallback for unknown types
    //    };

    //    // Return the file for download
    //    var bytes = Encoding.UTF8.GetBytes(template);
    //    var fileName = $"{templateType}-Template{fileExtension}";
    //    return File(bytes, mimeType, fileName);
    //}

    [HttpGet]
    public IActionResult DownloadTemplate(string templateType, string fileExtension)
    {
        // Identify the appropriate import service based on file extension
        var importService = _importServices.FirstOrDefault(service => service.CanHandle(fileExtension));

        if (importService == null)
        {
            return BadRequest("Unsupported file format.");
        }

        // Determine the MIME type dynamically
        var mimeType = fileExtension switch
        {
            ".csv" => "text/csv",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };

        var fileName = $"{templateType}-Template{fileExtension}";

        // Handle CSV (text-based)
        if (fileExtension == ".csv")
        {
            var template = importService.GenerateTemplate(templateType);
            if (string.IsNullOrWhiteSpace(template))
            {
                return StatusCode(500, "Failed to generate template.");
            }

            var bytes = Encoding.UTF8.GetBytes(template);
            return File(bytes, mimeType, fileName);
        }

        // Handle DOCX (binary)
        if (fileExtension == ".docx")
        {
            // Assuming GenerateTemplate(templateType) returns plain text content (lines to insert into docx)
            var content = importService.GenerateTemplate(templateType);
            if (string.IsNullOrWhiteSpace(content))
            {
                return StatusCode(500, "Failed to generate template.");
            }

            byte[] docxBytes;

            using (var ms = new MemoryStream())
            {
                using (var wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    var mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new Document(new Body());

                    var body = mainPart.Document.Body;

                    foreach (var line in content.Split('\n'))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var paragraph = new Paragraph(new Run(new Text(line.Trim())));
                            body.Append(paragraph);
                        }
                    }

                    mainPart.Document.Save();
                }

                docxBytes = ms.ToArray();
            }

            return File(docxBytes, mimeType, fileName);
        }

        return BadRequest("Unsupported file type.");
    }


    //// GET: CreateManualExam
    //[HttpGet]
    //public async Task<IActionResult> CreateManualExam()
    //{
    //    var professorId = User.GetProfessorId();

    //    var filterData = await _questionService.GetQuestionsFilteredAdvancedAsync(
    //        professorId,
    //        new List<int>(), new List<int>(), null, null, new List<int>());

    //    var viewModel = new CreateManualExamViewModel
    //    {
    //        Questions = _mapper.Map<List<QuestionViewModel>>(filterData.Questions),

    //        Categories = filterData.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
    //        Courses = new List<SelectListItem>(), // will be loaded dynamically
    //        Lectures = new List<SelectListItem>(), // will be loaded dynamically
    //        QuestionTypes = filterData.QuestionTypes.Select(qt => new SelectListItem { Value = qt.Id.ToString(), Text = qt.Type }).ToList(),
    //        DifficultyLevels = filterData.DifficultyLevels.Select(dl => new SelectListItem { Value = dl.Id.ToString(), Text = dl.Name }).ToList(),

    //        SelectedQuestions = new List<SelectedQuestionDto>(),
    //        SelectedLectureIds = new List<int>(),
    //        SelectedDifficultyLevelIds = new List<int>(),
    //        SelectedQuestionTypeIds = new List<int>()
    //    };

    //    return View(viewModel);
    //}

    //[HttpGet]
    //public async Task<IActionResult> CreateManualExam(
    //int? selectedCategoryId,
    //int? selectedCourseId,
    //List<int>? selectedLectureIds,
    //List<int>? selectedDifficultyLevelIds,
    //List<int>? selectedQuestionTypeIds)
    //{
    //    var professorId = User.GetProfessorId();

    //    var filterData = await _questionService.GetQuestionsFilteredAdvancedAsync(
    //        professorId,
    //        selectedQuestionTypeIds ?? new List<int>(),
    //        selectedDifficultyLevelIds ?? new List<int>(),
    //        selectedCategoryId,
    //        selectedCourseId,
    //        selectedLectureIds ?? new List<int>());

    //    var viewModel = new CreateManualExamViewModel
    //    {
    //        ExamTitle = "", // default empty
    //        DurationMinutes = null,
    //        StartTime = null,

    //        SelectedCategoryId = selectedCategoryId,
    //        SelectedCourseId = selectedCourseId,
    //        SelectedLectureIds = selectedLectureIds ?? new List<int>(),
    //        SelectedDifficultyLevelIds = selectedDifficultyLevelIds ?? new List<int>(),
    //        SelectedQuestionTypeIds = selectedQuestionTypeIds ?? new List<int>(),

    //        Questions = _mapper.Map<List<QuestionViewModel>>(filterData.Questions)
    //               .OrderBy(q => q.QuestionTypeName)
    //               .ThenBy(q => q.DifficultyLevelName)
    //               .ThenBy(q => q.LectureName)
    //               .ToList(),
    //        Categories = filterData.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
    //        Courses = filterData.Courses.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
    //        Lectures = filterData.Lectures.Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.LectureName }).ToList(),
    //        QuestionTypes = filterData.QuestionTypes.Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Type }).ToList(),
    //        DifficultyLevels = filterData.DifficultyLevels.Select(dl => new SelectListItem { Value = dl.Id.ToString(), Text = dl.Name }).ToList(),
    //        SelectedQuestions = new List<SelectedQuestionDto>()
    //    };

    //    return View(viewModel);
    //}
    //[HttpGet]
    //public async Task<IActionResult> CreateManualExam(
    //int? selectedCategoryId,
    //int? selectedCourseId,
    //List<int>? selectedLectureIds,
    //List<int>? selectedDifficultyLevelIds,
    //List<int>? selectedQuestionTypeIds,
    //bool isAjaxRequest = false) // This flag determines AJAX filtering
    //{
    //    var professorId = User.GetProfessorId();

    //    var filterData = await _questionService.GetQuestionsFilteredAdvancedAsync(
    //        professorId,
    //        selectedQuestionTypeIds ?? new List<int>(),
    //        selectedDifficultyLevelIds ?? new List<int>(),
    //        selectedCategoryId,
    //        selectedCourseId,
    //        selectedLectureIds ?? new List<int>());

    //    var viewModel = new CreateManualExamViewModel
    //    {
    //        ExamTitle = "", // default empty
    //        DurationMinutes = null,
    //        StartTime = null,

    //        //SelectedCategoryId = selectedCategoryId,
    //     //   SelectedCourseId = selectedCourseId,
    //     ///   SelectedLectureIds = selectedLectureIds ?? new List<int>(),
    //        //SelectedDifficultyLevelIds = selectedDifficultyLevelIds ?? new List<int>(),
    //        //SelectedQuestionTypeIds = selectedQuestionTypeIds ?? new List<int>(),

    //        Questions = _mapper.Map<List<QuestionViewModel>>(filterData.Questions)
    //            .OrderBy(q => q.QuestionTypeName)
    //            .ThenBy(q => q.DifficultyLevelName)
    //            .ThenBy(q => q.LectureName)
    //            .ToList(),
    //        Categories = filterData.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
    //     //  Courses = filterData.Courses.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
    //     //   Lectures = filterData.Lectures.Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.LectureName }).ToList(),
    //        QuestionTypes = filterData.QuestionTypes.Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Type }).ToList(),
    //        DifficultyLevels = filterData.DifficultyLevels.Select(dl => new SelectListItem { Value = dl.Id.ToString(), Text = dl.Name }).ToList(),
    //        SelectedQuestions = new List<SelectedQuestionDto>()
    //    };

    //    // If it's an AJAX request, return only the filtered questions
    //    if (isAjaxRequest)
    //    {
    //        return PartialView("_FilteredQuestionsPartial", viewModel.Questions);
    //    }

    //    // Otherwise, return the full page view
    //    return View(viewModel);
    //}
    //[HttpPost]
    //public IActionResult PreviewManualExam(CreateManualExamViewModel model)
    //{
    //    // Validate model and selected questions
    //    if (model.SelectedQuestions == null || !model.SelectedQuestions.Any())
    //    {
    //        TempData["Error"] = "No questions selected.";
    //        return RedirectToAction("CreateManualExam");
    //    }

    //    // Maybe fetch full question details again if needed
    //    // You can also prepare a special ViewModel for the preview page

    //    return View("ManualExamPreview", model); // Return a detailed preview page
    //}
    [HttpPost]
    public async Task<IActionResult> PreviewExam(PreviewExamViewModel model, string action)
    {
        //if (model.SelectedQuestions == null || !model.SelectedQuestions.Any())
        //{
        //    ModelState.AddModelError("", "No questions selected.");
        //    return RedirectToAction("CreateManualExam");
        //}
        var professorId = User.GetProfessorId();

        model.ProfessorId=professorId;

        // Collect all QuestionIds into a list
        var selectedQuestionIds = model.SelectedQuestions.Select(q => q.QuestionId).ToList();

        // Fetch all answers asynchronously in one query
        var answersList = await _questionService.GetAnswersForQuestionsAsync(selectedQuestionIds);

        // Enrich selected questions with their answers
        model.SelectedQuestions = model.SelectedQuestions.Select(selected => new SelectedQuestionDto
        {
            QuestionId = selected.QuestionId,
            QuestionText = selected.QuestionText,
            Points = selected.Points,
            LectureName = selected.LectureName,
            QuestionTypeName = selected.QuestionTypeName,
            OrderIndex = selected.OrderIndex, // ✅ Keep this

            DifficultyLevelName = selected.DifficultyLevelName,

            Answers = answersList.Where(a => a.QuestionId == selected.QuestionId).ToList()
        }).ToList();
        switch (action)
        {
            case "save":
                var examDto = _mapper.Map<ExamAddDto>(model);
                await _examService.AddExamAsync(examDto);
                return RedirectToAction("ExamList");

            case "back":
                return RedirectToAction("ExamList");

            default:
                return View("PreviewManualExam", model);
        }

       
    }



    [HttpPost]
    public IActionResult DownloadExamPdf(CreateManualExamViewModel model)
    {
        
        return new ViewAsPdf("PreviewManualExam", model)
        {
            FileName = "ExamPreview.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            CustomSwitches = "--disable-smart-shrinking --print-media-type --page-offset 0 --margin-bottom 25mm"
        };
    }
    //public IActionResult SomeView()
    //{
    //    ViewBag.IsPdf = false; // Default view rendering
    //    return View();
    //}
    [HttpGet]
    public async Task<IActionResult> CreateManualExam(
    bool isAjaxRequest = false,
    int? selectedCategoryId = null,
    int? selectedCourseId = null,
    List<int>? selectedLectureIds = null,
    List<int>? selectedDifficultyLevelIds = null,
    List<int>? selectedQuestionTypeIds = null)
    {
        var professorId = User.GetProfessorId();

        // If it's an AJAX request, fetch only filtered questions
        if (isAjaxRequest)
        {
            var filteredQuestions = await _questionService.GetQuestionsFilteredAdvancedAsync(
                professorId,
                selectedQuestionTypeIds ?? new List<int>(),
                selectedDifficultyLevelIds ?? new List<int>(),
                selectedCategoryId,
                selectedCourseId,
                selectedLectureIds ?? new List<int>());

            var mappedQuestions = _mapper.Map<List<QuestionViewModel>>(filteredQuestions.Questions)
                .OrderBy(q => q.QuestionTypeName)
                .ThenBy(q => q.DifficultyLevelName)
                .ThenBy(q => q.LectureName)
                .ToList();

            return PartialView("_GeneratedQuestionsPartial", mappedQuestions);
        }

        // First-time page load: Fetch filter options (categories, question types, difficulty levels)
        var filterData = await _questionService.GetFilterOptionsAsync(professorId);

        var viewModel = new CreateManualExamViewModel
        {
            //ExamTitle = "",
            //DurationMinutes = null,
            //StartTime = null,
            //Questions = new List<QuestionViewModel>(),  // Empty on first load
            Categories = filterData.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
            Courses = filterData.Courses.Select(c => new SelectListItem {Value = c.Id.ToString(),Text = c.Name }) .ToList(),
            QuestionTypes = filterData.QuestionTypes.Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Type }).ToList(),
            DifficultyLevels = filterData.DifficultyLevels.Select(dl => new SelectListItem { Value = dl.Id.ToString(), Text = dl.Name }).ToList(),
            SelectedQuestions = new List<SelectedQuestionDto>()
        };

        return View(viewModel);
    }


    //[HttpGet]
    //public async Task<IActionResult> CreateAutoExam()
    //{
    //    var professorId = User.GetProfessorId();
    //    var options = await _questionService.GetFilterOptionsAsync(professorId);

    //    var viewModel = new CreateAutoExamViewModel
    //    {
    //        Categories = options.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
    //        QuestionTypes = options.QuestionTypes.Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Type }).ToList(),
    //        DifficultyLevels = options.DifficultyLevels.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name }).ToList(),
    //        Courses = options.Courses.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
    //    };

    //    return View(viewModel);
    //}
    //[HttpGet]
    //public async Task<IActionResult> CreateAutoExam( CreateAutoExamViewModel model, bool isAjaxRequest = false)
    //{
    //    var professorId = User.GetProfessorId();

    //    // Generate questions if it's an AJAX request
    //    if (isAjaxRequest)
    //    {
    //        var requestDto = _mapper.Map<AutoExamGenerationRequestDto>(model);
    //        requestDto.ProfessorId = professorId;

    //        var generatedQuestions = await _questionService.GenerateQuestionsBasedOnCountsAsync(requestDto);

    //        var mappedQuestions = _mapper.Map<List<QuestionViewModel>>(generatedQuestions.Questions)
    //            .OrderBy(q => q.QuestionTypeName)
    //            .ThenBy(q => q.DifficultyLevelName)
    //            .ThenBy(q => q.LectureName)
    //            .ToList();

    //        return PartialView("_GeneratedQuestionsPartial", mappedQuestions);
    //    }

    //    // Fetch filter options for the initial page load
    //    var filterData = await _questionService.GetFilterOptionsAsync(professorId);

    //    var viewModel = new CreateAutoExamViewModel
    //    {
    //        //ExamTitle = "",
    //        Categories = filterData.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
    //        //Courses = filterData.Courses.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
    //    };

    //    return View(viewModel);
    //}

    [HttpGet]
    public async Task<IActionResult> CreateAutoExam(bool isAjaxRequest = false,
    int? selectedCategoryId = null,
    int? selectedCourseId = null,
    List<int>? selectedLectureIds = null,
    int? totalMcq = null,
    int? mcqEasy = null,
    int? mcqMedium = null,
    int? mcqHard = null,
    int? totalTf = null,
    int? tfEasy = null,
    int? tfMedium = null,
    int? tfHard = null)
    {
        var professorId = User.GetProfessorId();

        if (isAjaxRequest)
        {
            var requestDto = new AutoExamGenerationRequestDto
            {
                ProfessorId = professorId,
                SelectedCategoryId = selectedCategoryId ,  // Ensures it's never null
                SelectedCourseId = selectedCourseId ,
                SelectedLectureIds = selectedLectureIds ?? new List<int>(),
                McqEasy = mcqEasy ?? 0,
                McqMedium = mcqMedium ?? 0,
                McqHard = mcqHard ?? 0,
                TfEasy = tfEasy ?? 0,
                TfMedium = tfMedium ?? 0,
                TfHard = tfHard ?? 0
            };


            var generatedQuestions = await _questionService.GenerateQuestionsBasedOnCountsAsync(requestDto);

            var mappedQuestions = _mapper.Map<List<QuestionViewModel>>(generatedQuestions.Questions)
                .OrderBy(q => q.QuestionTypeName)
                .ThenBy(q => q.DifficultyLevelName)
                .ThenBy(q => q.LectureName)
                .ToList();

            return PartialView("_GeneratedQuestionsPartial", mappedQuestions);
        }

        // Fetch filter options for the initial page load
        var filterData = await _questionService.GetFilterOptionsAsync(professorId);

        var viewModel = new CreateAutoExamViewModel
        {
            Categories = filterData.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
            Courses = filterData.Courses.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

        };

        return View(viewModel);
    }


    [HttpGet]
    public async Task<IActionResult> ExamList()
    {
        var professorId = User.GetProfessorId();
        var exams = await _examService.GetExamListAsync(professorId);

        var viewModel = new ExamListViewModel
        {
            Exams = exams
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> ExamDetails(int id)
    {
        var professorId = User.GetProfessorId();
        var examDto = await _examService.GetExamByIdAsync(id, professorId);

        if (examDto == null)
        {
            return NotFound();
        }

        var viewModel = _mapper.Map<ExamDetailsViewModel>(examDto); // ✅ Automapped


        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteExam(int id)
    {
        await _examService.DeleteExamAsync(id);
        return RedirectToAction("ExamList");
    }
    [HttpGet]
    public async Task<IActionResult> GetAvailableQuestionsCount(
    int courseId,
    int? categoryId,
    List<int> lectureIds,      // this stays a list
    int questionTypeId,         // single value
    int difficultyId            // single value
)
    {
        var professorId = User.GetProfessorId();
        var count = await _questionService.CountQuestionsAsync(
            professorId,
            courseId,
            categoryId,
            lectureIds,
            questionTypeId,
            difficultyId
        );

        return Json(new { count });
    }

    [HttpGet]
    public async Task<IActionResult> DownloadExamPdf(int id)
    {
        var professorId = User.GetProfessorId();
        var examDto = await _examService.GetExamByIdAsync(id, professorId);

        if (examDto == null)
            return NotFound();

        var viewModel = _mapper.Map<ExamDetailsViewModel>(examDto);

        var fileName = $"Exam_{viewModel.CourseName}_{DateTime.Now:yyyyMMdd}.pdf";
        var pdf = new ViewAsPdf("ExamDetailsPdf", viewModel)
        {
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageMargins = new Rotativa.AspNetCore.Options.Margins { Top = 10, Bottom = 25, Left = 10, Right = 10 },
            CustomSwitches = "--disable-smart-shrinking --print-media-type --page-offset 0"
        };

        // Set UTF-8 filename header to support Arabic
        Response.Headers["Content-Disposition"] = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(fileName);

        return pdf;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateExamPdfs(int examId, int copyCount = 1)
    {
        var professorId = User.GetProfessorId();
        var examDto = await _examService.GetExamByIdAsync(examId, professorId);

        if (examDto == null)
            return NotFound();

        if (copyCount < 1 || copyCount > 50)
        {
            TempData["ErrorMessage"] = "Number of copies must be between 1 and 50.";
            return RedirectToAction("ExamDetails", new { id = examId });
        }

        var viewModel = _mapper.Map<ExamDetailsViewModel>(examDto);

        // Store the data in TempData for the generation page
        TempData["ExamViewModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel);
        TempData["CopyCount"] = copyCount;

        return RedirectToAction("GenerateExamPdfsPage");
    }

    [HttpGet]
    public IActionResult GenerateExamPdfsPage()
    {
        var viewModelJson = TempData["ExamViewModel"] as string;
        var copyCount = TempData["CopyCount"] as int?;

        if (string.IsNullOrEmpty(viewModelJson) || !copyCount.HasValue)
            return RedirectToAction("ExamList");

        var viewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ExamDetailsViewModel>(viewModelJson);

        ViewBag.CopyCount = copyCount.Value;
        ViewBag.CourseName = viewModel.CourseName;

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadShuffledExamPdf(int examId, int copyNumber, bool isAnswerKey = false)
    {
        var professorId = User.GetProfessorId();
        var examDto = await _examService.GetExamByIdAsync(examId, professorId);

        if (examDto == null)
            return NotFound();

        var viewModel = _mapper.Map<ExamDetailsViewModel>(examDto);

        // Shuffle questions reproducibly
        var random = new Random(examId * 1000 + copyNumber);
        var shuffledQuestions = viewModel.Questions.OrderBy(x => random.Next()).ToList();
        viewModel.Questions = shuffledQuestions
            .OrderBy(q => q.QuestionType == "True/False") // Multiple Choice first
            .ToList();

        var viewName = isAnswerKey ? "ExamAnswerKeyPdf" : "ExamDetailsPdf";
        var filePrefix = isAnswerKey ? "Answer_Key" : "Exam_Paper";

        var fileName = $"{filePrefix}_Copy_{copyNumber}_{viewModel.CourseName}_{DateTime.Now:yyyyMMdd}.pdf";
        var pdf = new ViewAsPdf(viewName, viewModel)
        {
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageMargins = new Rotativa.AspNetCore.Options.Margins { Top = 15, Bottom = 15, Left = 15, Right = 15 },
            CustomSwitches = "--disable-smart-shrinking --print-media-type"
        };

        // Set UTF-8 filename header
        Response.Headers["Content-Disposition"] = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(fileName);

        return pdf;
    }


    [HttpGet]
    public async Task<IActionResult> AutoGenerateQeustion()
      
    {
        // Get professorId from logged-in user
        var professorId = User.GetProfessorId();

        // Call the service to get courses with lectures and files
        var courses = await _courseService.GetCoursesWithLecturesAndFilesByProfessorAsync(professorId);

        // Map to ViewModel if needed, or pass DTOs directly
        var model = courses.Select(c => new CourseOptionViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Lectures = c.Lectures.Select(l => new LectureOptionViewModel
            {
                Id = l.Id,
                LectureName = l.LectureName,
                Files = l.Files.Select(f => new LectureFileOptionViewModel
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FilePath = f.FilePath
                }).ToList()
            }).ToList()
        }).ToList();

        return View(model); // or return Json(model) for API
    }

    //[HttpPost]
    //public async Task<IActionResult> GenerateQuestions(IFormFile lecturePdf, Dictionary<string, int> parameters)
    //{
    //    if (lecturePdf == null || lecturePdf.Length == 0)
    //        return BadRequest("No file uploaded.");

    //    string filePath = Path.Combine(_env.WebRootPath, "uploads", lecturePdf.FileName);
    //    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

    //    using (var stream = new FileStream(filePath, FileMode.Create))
    //    {
    //        await lecturePdf.CopyToAsync(stream);
    //    }

    //    string result = await _questionGenerationService.GenerateQuestionsAsync(filePath, parameters);
    //    return Content(result, "text/plain");
    //}
    [HttpPost]
    public async Task<IActionResult> GenerateQuestions([FromBody] QuestionSettingsViewModel model)
    {
        if (model == null || model.FilesToProcess.Count == 0)
            return BadRequest("No lectures selected.");

        try
        {
            var fileIds = model.FilesToProcess.Select(f => f.FileId).ToList();

            string aiResponse = await _ragService.GenerateQuestionsAsync(
                fileIds,
                model.Distribution,
                model.SemanticQuery
            );

            var parsedDto = _questionParser.Parse(aiResponse);
            var viewModel = _mapper.Map<GeneratedQuestionsViewModel>(parsedDto);

            viewModel.CourseId = model.CourseId;

            // ✅ Save ViewModel in Session instead of TempData
            HttpContext.Session.SetString(
                "GeneratedQuestions",
                System.Text.Json.JsonSerializer.Serialize(viewModel)
            );

            // Return URL instead of HTML
            return Json(new { redirectUrl = Url.Action("QuestionResultsPage", "Professor") });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }

    }

    public IActionResult QuestionResultsPage()
    {
        // ✅ Check if data exists in Session instead of TempData
        var json = HttpContext.Session.GetString("GeneratedQuestions");
        if (string.IsNullOrEmpty(json))
            return RedirectToAction("Index"); // fallback if nothing stored

        var viewModel = System.Text.Json.JsonSerializer.Deserialize<GeneratedQuestionsViewModel>(json);

        return View("QuestionResults", viewModel);
    }



    //[HttpPost]
    //public async Task<IActionResult> GenerateQuestions([FromBody] QuestionSettingsViewModel model)
    //{
    //    if (model == null || model.FilesToProcess.Count == 0)
    //        return BadRequest("No lectures selected.");

    //    try
    //    {
    //        // Collect lecture IDs
    //        var fileIds = model.FilesToProcess.Select(f => f.FileId).ToList();


    //        // Call RAG backend through RagService
    //        string aiResponse = await _ragService.GenerateQuestionsAsync(
    //            fileIds,
    //            model.Distribution,
    //            model.SemanticQuery
    //        );

    //            var parsedDto = _questionParser.Parse(aiResponse);
    //            var viewModel = _mapper.Map<GeneratedQuestionsViewModel>(parsedDto);

    //            viewModel.CourseId = model.CourseId;

    //        return View("QuestionResults", viewModel);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Error generating questions: {ex.Message}");
    //    }
    //}

    //[HttpPost]
    //public async Task<IActionResult> GenerateQuestion([FromBody] QuestionSettingsViewModel model)
    //{
    //    if (model == null || model.LecturesToProcess.Count == 0)
    //        return BadRequest("No lectures selected.");



    //    string aiResponse = await _questionGenerationService.GenerateQuestionsAsync(
    //        model.CourseId,
    //        model.LecturesToProcess.Select(l => l.LectureId).ToList(),
    //        model.Distribution,
    //        model.SemanticQuery
    //    );

    //    var parsedDto = _questionParser.Parse(aiResponse);
    //    var viewModel = _mapper.Map<GeneratedQuestionsViewModel>(parsedDto);

    //    return Ok();
    //}


    [HttpPost]
    public async Task<IActionResult> SaveGeneratedQuestions(GeneratedQuestionsViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("QuestionResults", viewModel);
        }

        var professorId = User.GetProfessorId();

        // Map ViewModel to your Business DTO - e.g., ParsedQuestionsDto
        var parsedDto = _mapper.Map<ParsedQuestionsDto>(viewModel);

        await _questionService.SaveGeneratedQuestionsAsync(parsedDto, professorId);

        TempData["Success"] = "Questions saved successfully!";

        return RedirectToAction("ManageQuestions", "Professor");
    }





    //[HttpGet]
    //public IActionResult QuestionResults()
    //{
    //    if (!TempData.ContainsKey("GeneratedQuestions"))
    //        return RedirectToAction("Generate"); // or wherever appropriate

    //    var json = TempData["GeneratedQuestions"] as string;
    //    var parsedDto = JsonConvert.DeserializeObject<GeneratedQuestionsViewModel>(json);

    //    // Keep TempData for next request as well
    //    TempData.Keep("GeneratedQuestions");

    //    var viewModel = _mapper.Map<GeneratedQuestionsViewModel>(parsedDto);
    //    return View(viewModel);
    //}
    [HttpPost]
    public async Task<IActionResult> DeleteeQuestion(Guid questionId)
    {
        var dtoJson = TempData["GeneratedQuestions"] as string;
        if (string.IsNullOrEmpty(dtoJson))
            return BadRequest();

        var parsedDto = JsonConvert.DeserializeObject<GeneratedQuestionsViewModel>(dtoJson);

        foreach (var group in parsedDto.QuestionGroups)
            group.Questions = group.Questions.Where(q => q.Id != questionId).ToList();

        TempData["GeneratedQuestions"] = JsonConvert.SerializeObject(parsedDto);

        return Ok(); // AJAX-friendly response
    }

    //[HttpPost]
    //public async Task<IActionResult> SaveGeneratedQuestions()
    //{
    //    var dtoJson = TempData["GeneratedQuestions"] as string;
    //    if (string.IsNullOrEmpty(dtoJson))
    //        return RedirectToAction("QuestionResults");

    //    var parsedDto = JsonConvert.DeserializeObject<GeneratedQuestionsViewModel>(dtoJson);
    //    var professorId = User.GetProfessorId(); // 👍 Use your existing method

    //    foreach (var group in parsedDto.QuestionGroups)
    //    {
    //        foreach (var question in group.Questions)
    //        {
    //            var viewModel = new QuestionAddViewModel
    //            {
    //                Text = question.QuestionText,
    //                QuestionTypeId = MapToTypeId(group.Type), // 👈 Implement mapping if needed
    //                DifficultyLevelId = 2, // Set default or derive from UI
    //                professorId = professorId,
    //                CategoryId = 1, // Or assign dynamically
    //                CourseId = 1, // Same here
    //                LectureName = "Generated Lecture", // Or source from context

    //                Answers = question.Choices.Select((c, i) => new AnswerAddViewModel
    //                {
    //                    Text = c,
    //                    IsCorrect = c == question.Answer // ✅ Match correctness
    //                }).ToList(),
    //                CorrectAnswerIndex = question.Choices.FindIndex(c => c == question.Answer)
    //            };

    //            var questionDto = _mapper.Map<QuestionAddDto>(viewModel);
    //            await _questionService.AddQuestionAsync(questionDto);
    //        }
    //    }

    //    TempData["SuccessMessage"] = "🟢 All questions have been saved successfully.";
    //    return RedirectToAction("QuestionResults");
    //}

    //private int MapToTypeId(string type)
    //{
    //    return type.ToLower() switch
    //    {
    //        "multiplechoice" => 1,
    //        "truefalse" => 2,
    //        _ => 0 // Default or throw
    //    };
    //}











    //[HttpPost]
    //public IActionResult DeleteGeneratedQuestion(string type, string questionText)
    //{
    //    if (!TempData.ContainsKey("GeneratedQuestions"))
    //        return RedirectToAction("QuestionResults");

    //    var json = TempData["GeneratedQuestions"] as string;
    //    var parsedDto = JsonConvert.DeserializeObject<GeneratedQuestionsViewModel>(json);

    //    var group = parsedDto.QuestionGroups.FirstOrDefault(g => g.Type == type);
    //    if (group != null)
    //    {
    //        var question = group.Questions.FirstOrDefault(q => q.QuestionText == questionText);
    //        if (question != null)
    //            group.Questions.Remove(question);
    //    }

    //    TempData["GeneratedQuestions"] = JsonConvert.SerializeObject(parsedDto);
    //    TempData.Keep("GeneratedQuestions");

    //    return RedirectToAction("QuestionResults");
    //}

    //[HttpPost]
    //public async Task<IActionResult> SaveGeneratedQuestions()
    //{
    //    if (!TempData.ContainsKey("GeneratedQuestions"))
    //        return BadRequest("No questions to save.");

    //    var json = TempData["GeneratedQuestions"] as string;
    //    var parsedDto = JsonConvert.DeserializeObject<GeneratedQuestionsViewModel>(json);

    //    // Your saving logic here:
    //  //  await _questionService.SaveGeneratedQuestionsAsync(parsedDto);

    //    TempData.Remove("GeneratedQuestions");

    //    return RedirectToAction("SaveSuccess"); // Or wherever you want
    //}

    //public IActionResult SaveSuccess()
    //{
    //    return View(); // Show success message
    //}

    [HttpGet]
    public async Task<IActionResult> UploadLectureFile()
    {
        var professorId = User.GetProfessorId();
        var courses = await _courseService.GetCoursesWithAvailableLecturesByProfessorAsync(professorId);

        var model = new LectureUploadViewModel
        {
            Courses = courses.Select(c => new CourseOptionViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Lectures = c.Lectures.Select(l => new LectureOptionViewModel
                {
                    Id = l.Id,
                    LectureName = l.LectureName,
                    FileNames = l.FileNames // pass existing files to view
                }).ToList()
            }).ToList()
        };


        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> UploadLectureFile(LectureUploadViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.File == null || model.File.Length == 0)
        {
            ModelState.AddModelError("File", "Please select a file.");
            return View(model);
        }

        // Get professorId from logged-in user
        var professorId = User.GetProfessorId(); // Assuming you have an extension method

        try
        {
            // Call your service layer — automatically creates lecture if missing
            await _lectureFileService.SaveLectureFileToDiskAsync(
                courseName: model.SelectedCourseName,
                lectureName: model.SelectedLectureName,
                courseId: model.SelectedCourseId,
                professorId: professorId,
                file: model.File
            );

            TempData["SuccessMessage"] = "Lecture file uploaded successfully!";
            return RedirectToAction("ViewUploadedLectures");
        }
        catch (InvalidOperationException ex)
        {
            // Show error in the view
            ModelState.AddModelError("File", ex.Message);
            return View(model);
        }
    }


    [HttpGet]
    public async Task<IActionResult> ViewUploadedLectures()
    {
        // Get professorId from logged-in user
        var professorId = User.GetProfessorId();

        // Call the service to get courses with lectures and files
        var courses = await _courseService.GetCoursesWithLecturesAndFilesByProfessorAsync(professorId);

        // Map to ViewModel if needed, or pass DTOs directly
        var model = courses.Select(c => new CourseOptionViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Lectures = c.Lectures.Select(l => new LectureOptionViewModel
            {
                Id = l.Id,
                LectureName = l.LectureName,
                Files = l.Files.Select(f => new LectureFileOptionViewModel
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FilePath = f.FilePath
                }).ToList()
            }).ToList()
        }).ToList();

        return View(model); // or return Json(model) for API
    }

    [HttpPost]
    public async Task<IActionResult> DeleteLectureFile(int lectureFileId, bool deleteQuestions)
    {
        try
        {
            // Change your service method to accept lectureFileId
            await _lectureFileService.DeleteLectureFileAsync(lectureFileId, deleteQuestions);

            //TempData["SuccessMessage"] = "Deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error: " + ex.Message;
        }

        return RedirectToAction("ViewUploadedLectures");
    }




}
