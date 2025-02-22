using AutoMapper;
using BusinessLogicLayer.Dtos;
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

        public AdminController(IUserService userService, IMapper mapper, IHistoryService historyService, IRoleService roleService)
        {
            _userService = userService;
            _mapper = mapper;
            _historyService = historyService;
            _roleService = roleService;


        }
        [HttpGet]
        public async Task<IActionResult> UserManagement(int? roleId)
        {
            // Fetching all users based on the roleId filter
            var users = await _userService.GetUsersFilteredAsync(roleId);

            // Map the result to a ViewModel (or DTO) for use in the View
            var userDtos = _mapper.Map<IEnumerable<UserViewModel>>(users);

            // Fetching filter options (roles)
            var roles = await _roleService.GetAllRolesAsync(); // Fetch all roles for the filter

            // Setting up the ViewBag for use in the View (populating dropdown list)
            ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);

            // Return the View with the populated ViewModel
            return View(userDtos);
        }


        // Create user (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
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
                await _userService.RegisterUserAsync(registrationDto);

                TempData["SuccessMessage"] = " Professor add successfully.";
                return RedirectToAction("UserManagement");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Email", ex.Message); // Add error to the specific field
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
    }
}
