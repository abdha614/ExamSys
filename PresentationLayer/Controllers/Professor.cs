using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.QuestionDto;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Models;
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


[Authorize(Policy = "ProfessorPolicy")]
public class ProfessorController : Controller
{
    private readonly IQuestionService _questionService;
    private readonly ICategoryService _categoryService;
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;
    private readonly IQuestionTypeService _questionTypeService;
    private readonly ICsvImportService _csvImportService;

    public ProfessorController(IQuestionService questionService, ICategoryService categoryService, ICourseService courseService, IQuestionTypeService questionTypeService, IMapper mapper, ICsvImportService csvImportService)
    {
        _questionService = questionService;
        _categoryService = categoryService;
        _courseService = courseService;
        _questionTypeService = questionTypeService;
        _mapper = mapper;
        _csvImportService = csvImportService;
    }
    // ManageQuestions action to display list of questions for the professor
    public async Task<IActionResult> ManageQuestions(int? questionTypeId = null, int? difficultyLevelId = null, int? categoryId = null, int? courseId = null)
    {
        var professorId = User.GetProfessorId(); // Custom extension to get professor ID from User claims
        var questions = await _questionService.GetQuestionsFilteredAsync(professorId, questionTypeId, difficultyLevelId, categoryId, courseId);
        var viewModel = _mapper.Map<IEnumerable<QuestionViewModel>>(questions);

        // Fetching filter options
        var categories = await _categoryService.GetCategoriesByProfessorAsync(professorId);
        var courses = await _courseService.GetCoursesByProfessorAsync(professorId);
        var questionTypes = await _questionTypeService.GetAllQuestionTypesAsync(); // Fetching all QuestionTypes as DTOs

        ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
        ViewBag.Courses = new SelectList(courses, "Id", "Name", courseId);
        ViewBag.QuestionTypes = new SelectList(questionTypes, "Id", "Type", questionTypeId); // Convert question types to SelectList
        ViewBag.SelectedDifficultyLevelId = difficultyLevelId;
        ViewBag.SelectedCourseId = courseId;

        // Set error message only if filters are applied and no questions are found
        bool isFilterApplied = questionTypeId.HasValue || difficultyLevelId.HasValue || categoryId.HasValue || courseId.HasValue;
        if (isFilterApplied && !viewModel.Any())
        {
            ViewBag.ErrorMessage = "No questions found for the selected filters.";
        }
        return View(viewModel);
    }

    public IActionResult ChooseQuestionType()
    {
        return View();
    }

    public async Task<IActionResult> AddMultipleChoiceQuestion()
    {
        var professorId = User.GetProfessorId();

        // Initializing the model with a single answer (or an empty list)
        var model = new QuestionAddViewModel
        {
            QuestionTypeId = 1, // Set QuestionTypeId for multiple-choice question
            Answers = new List<AnswerAddViewModel>
            {
            new AnswerAddViewModel(),
            new AnswerAddViewModel(),
            new AnswerAddViewModel() 
            }
        };

        var categories = await _categoryService.GetCategoriesByProfessorAsync(professorId);
        var courses = await _courseService.GetCoursesByProfessorAsync(professorId);
        ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
        ViewBag.Courses = new SelectList(courses, "Id", "Name", model.CourseId);

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

        // If the form is not valid, reload categories and return the same view
        var categories = await _categoryService.GetCategoriesByProfessorAsync(model.professorId);
        var courses = await _courseService.GetCoursesByProfessorAsync(model.professorId);
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.Courses = new SelectList(courses, "Id", "Name");

        return View(model);
    }

    public async Task<IActionResult> AddTrueFalseQuestion()
    {
        var professorId = User.GetProfessorId();
        // Initializing the model
        var model = new QuestionAddViewModel
        {
            QuestionTypeId = 2, // Set QuestionTypeId for True/False question
            Answers = new List<AnswerAddViewModel>
            {
                new AnswerAddViewModel { Text = "True" }, // Default True answer
                new AnswerAddViewModel { Text = "False" } // Default False answer
            }
        };

        var categories = await _categoryService.GetCategoriesByProfessorAsync(professorId);
        var courses = await _courseService.GetCoursesByProfessorAsync(professorId);
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.Courses = new SelectList(courses, "Id", "Name");

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> AddTrueFalseQuestion(QuestionAddViewModel model)
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

        // If the form is not valid, reload categories and return the same view
        var categories = await _categoryService.GetCategoriesByProfessorAsync(model.professorId);
        var courses = await _courseService.GetCoursesByProfessorAsync(model.professorId);
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.Courses = new SelectList(courses, "Id", "Name");

        return View(model);
    }

    // Action to return courses based on selected category and professor ID (for dynamic dropdown)
    [HttpGet]
    public async Task<JsonResult> GetCoursesByCategory(int categoryId)
    {
        var professorId = User.GetProfessorId();
        var category = await _categoryService.GetCategoryByIdAsync(categoryId);
        if (category == null || category.professorId != professorId)
        {
            return Json(new List<CourseGetDto>());
        }

        var courses = await _courseService.GetAllCoursesByCategoryAndProfessorAsync(category.Name, professorId);
        var coursesDto = _mapper.Map<IEnumerable<CourseGetDto>>(courses);
        return Json(coursesDto);
    }

    // Action to add a new category with professor ID
    [HttpPost]
    public async Task<IActionResult> AddCategory(string name)
    {
        var professorId = User.GetProfessorId();
       

        // Check if the category already exists for this professor
        var existingCategory = await _categoryService.GetCategoryByNameAndProfessorAsync(name, professorId);
        if (existingCategory != null)
        {
            return Json(new
            {
                error = true,
                errorMessage = "Category already exists for this professor."
            });
        }

        // Add the new category
        var category = await _categoryService.AddCategoryAsync(name, professorId);
        var categoryDto = _mapper.Map<CategoryGetDto>(category);

        return Json(categoryDto);
    }


    // Action to add a new course with professor ID
    [HttpPost]
    public async Task<IActionResult> AddCourse(string name, int categoryId)
    {
        var professorId = User.GetProfessorId();

        // Check for duplicates in the category for this professor
        var existingCourse = await _courseService.GetCourseByNameCategoryAndProfessorAsync(name, categoryId, professorId);
        if (existingCourse != null)
        {
            return Json(new
            {
                error = true,
                errorMessage = "Course already exists for this category."
            });
        }

        // Add the course if it does not already exist
        var course = await _courseService.AddCourseAsync(name, categoryId, professorId);

        return Json(new
        {
            error = false,
            data = course
        });
    }


    [HttpGet]
    public async Task<IActionResult> EditQuestion(int id)
    {
        var questionDto = await _questionService.GetQuestionByIdAsync(id);
        if (questionDto == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<QuestionUpdateViewModel>(questionDto);
        var categories = await _categoryService.GetCategoriesByProfessorAsync(questionDto.professorId);
        var courses = await _courseService.GetCoursesByProfessorAsync(questionDto.professorId); // Fetch courses for the dropdown

        ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
        ViewBag.Courses = new SelectList(courses, "Id", "Name", model.CourseId); // Pass the courses to the view

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

        var categories = await _categoryService.GetCategoriesByProfessorAsync(model.professorId);
        var courses = await _courseService.GetCoursesByProfessorAsync(model.professorId);

        ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
        ViewBag.Courses = new SelectList(courses, "Id", "Name", model.CourseId);

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
        var professorId = User.GetProfessorId(); // Get the current professor's ID

        // Check if the file is null or empty
        if (importFile == null || importFile.Length < 3)
        {
            TempData["ErrorMessage"] = "The uploaded file is empty. Please select a valid CSV file.";
            return RedirectToAction("ChooseQuestionType");
        }

        // Check if the file type is CSV
        var allowedExtensions = new[] { ".csv" };
        var fileExtension = Path.GetExtension(importFile.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
        {
            TempData["ErrorMessage"] = "Invalid file type. Please upload a CSV file.";
            return RedirectToAction("ChooseQuestionType");

        }

        try
        {
            using (var stream = importFile.OpenReadStream())
            {
                await _csvImportService.ImportQuestionsAsync(stream, importFile.FileName, professorId);
            }
            TempData["SuccessMessage"] = "Questions imported successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred while importing the file: {ex.Message}";
        }

        return RedirectToAction("ManageQuestions");
    }




}
