using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService; // Add IHistoryService to log user actions
        private readonly IUserService _userService;
      

        public AuthController(IAuthService authService,IUserService userService, IHistoryService historyService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
            _historyService = historyService;
            _userService = userService;
           
        }

        // Login action for existing users
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var loginDto = _mapper.Map<LoginDto>(loginViewModel);

            try
            {
                var userDto = await _authService.LoginAsync(loginDto);

                // Set claims for the user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()), // Ensure this claim is set
                    new Claim(ClaimTypes.Name, userDto.Email),
                    new Claim(ClaimTypes.Role, userDto.RoleId switch
                    {
                        1 => "Professor",
                        2 => "Student",
                        3 => "Admin",
                        _ => "Unknown"
                    })
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false // This ensures the cookie is a session cookie
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);


                // Role-based redirection
                return userDto.RoleId switch
                {
                    1 => RedirectToAction("ManageQuestions", "Professor"),
                    2 => RedirectToAction("Dashboard", "Student"),
                    3 =>    RedirectToAction("UserManagement", "Admin"),
                    _ => throw new UnauthorizedAccessException("Invalid role specified.")
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(loginViewModel);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Retrieve the current logged-in user's email from claims
            var email = User.Identity?.Name; // Ensure the email claim is set during login

            if (email != null)
            {
                // Fetch the user details by email
                var user = await _userService.GetUserByEmailAsync(email);
                if (user != null)
                {
                    // Log the logout action in the history table
                    await _historyService.LogActionAsync(new HistoryDto
                    {
                        UserId = user.Id,
                        Action = "User Logged Out",
                        IpAddress = "192.1.1.1",
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            // Clear authentication cookies
            Response.Cookies.Delete(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to login
            return RedirectToAction("Login");
        }

        //[HttpGet]
        //public IActionResult AccessDenied()
        //{
        //    return View();
        //}


        //// Debug user claims (temporary action for debugging purposes)
        //[Authorize]
        //[HttpGet]
        //public IActionResult DebugClaims()
        //{
        //    var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        //    return Json(userClaims);
        //}
    }
}
