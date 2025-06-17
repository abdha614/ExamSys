using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.CategoryDto;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]

  //  [Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;

        public AdminController(IUserService userService, IMapper mapper, IHistoryService historyService, IRoleService roleService, ICategoryService categoryService
 )
        {
            _userService = userService;
            _mapper = mapper;
            _historyService = historyService;
            _roleService = roleService;
            _categoryService = categoryService;


        }
        [HttpGet]
        public async Task<IActionResult> UserManagement(int? roleId)
        {
            // Fetch users and roles
            var users = await _userService.GetUsersFilteredAsync(roleId);
            var roles = await _roleService.GetAllRolesAsync();

            // Use AutoMapper to map users to view models
            var userDtos = _mapper.Map<IEnumerable<UserViewModel>>(users);

            // Manually map roles to SelectListItem
            var roleSelectList = roles.Select(role => new SelectListItem
            {
                Value = role.Id.ToString(),
                Text = role.Name,
                Selected = role.Id == roleId
            });

            // Construct the ViewModel
            var viewModel = new UserManagementViewModel
            {
                SelectedRoleId = roleId,
                Users = userDtos,
                Roles = roleSelectList
            };

            return View(viewModel);
        }
        // Create user (GET)
        public async Task<IActionResult> Register()
        {
            var roles = await _roleService.GetAllRolesAsync(); // returns List<Role> or similar

            var viewModel = new RegisterViewModel
            {
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                })
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            var registrationDto = _mapper.Map<UserRegistrationDto>(registerViewModel);

            try
            {
                var result = await _userService.RegisterUserAsync(registrationDto);
                TempData["SuccessMessage"] = $"Professor with email {result.Email} added successfully.";

                // If role is professor (e.g., roleId == 1), redirect to EditProfessorCategories
                if (result.RoleId == 1)
                {
                    return RedirectToAction("EditProfessorCategories", new { professorId = result.Id });
                }

                // Otherwise, go to the general user management page
                return RedirectToAction("UserManagement");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Email", ex.Message);
                TempData["ErrorMessage"] = "An account with this email already exists.";
                return RedirectToAction("Register");
            }
        }

        //Action to list the history
        [HttpGet]
        public async Task<IActionResult> UserHistory()
        {
            var historyData = await _historyService.GetHistoryAsync();

            var historyViewModels = _mapper.Map<List<HistoryViewModel>>(historyData);

            return View(historyViewModels);
        }

        //Delete user
       [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            TempData["SuccessMessage"] = " User Delete successfully.";
            return RedirectToAction(nameof(UserManagement));
        }
        [HttpGet]
        public async Task<IActionResult> ManageCategories()
        {
            // Fetch the categories with professor emails
            var categories = await _categoryService.GetAllCategoryWithProfessorEmailAsyncs();

            // Use AutoMapper to map to the view models if needed
            var categoryViewModels = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
            return View(categoryViewModels);
        }
        public async Task<IActionResult> EditProfessorCategories(int professorId)
        {
            var categories = await _categoryService.GetAllCategoryWithProfessorEmailAsyncs(professorId);

            var viewModel = new EditProfessorCategoriesViewModel
            {
                ProfessorId = professorId,
                Categories = categories.Select(c => new CategoryViewModel
                { 
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(EditProfessorCategoriesViewModel model)
        {
            var categoryExists = await _categoryService.GetCategoryByNameAndProfessorAsync(
                model.NewCategoryName, model.ProfessorId);

            if (categoryExists != null)
            {
                TempData["ErrorMessage"] = "The category already exists.";
                return RedirectToAction("EditProfessorCategories", new { professorId = model.ProfessorId });
            }

            // Use AutoMapper to map EditProfessorCategoriesViewModel to CategoryAddDto
            var categoryAddDto = _mapper.Map<CategoryAddDto>(model);

            await _categoryService.AddCategoryAsync(categoryAddDto);

            return RedirectToAction("EditProfessorCategories", new { professorId = model.ProfessorId });
        }



        [HttpPost]
        public async Task<IActionResult> RemoveCategory(int professorId, int categoryId)
        {
            await _categoryService.RemoveCategoryFromProfessorAsync(professorId, categoryId);
            return RedirectToAction("EditProfessorCategories", new { professorId });
        }


    }
}
