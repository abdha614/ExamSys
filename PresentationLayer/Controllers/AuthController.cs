using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.User_Management.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PresentationLayer.Hubs;
using PresentationLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IHistoryService _historyService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService; // Inject your email service
        private readonly IConfirmationCodeService _confirmationService; // For storing/verifying confirmation codes
        private readonly ISignupNotificationService _signupNotificationService;
        private readonly IHubContext<AdminNotificationHub> _hubContext;


        public AuthController(
            IAuthService authService,
            IUserService userService,
            IHistoryService historyService,
            IMapper mapper,
            IEmailService emailService,
            ISignupNotificationService signupNotificationService,
            IConfirmationCodeService confirmationService,
            IHubContext<AdminNotificationHub> hubContext)

        {
            _authService = authService;
            _userService = userService;
            _historyService = historyService;
            _mapper = mapper;
            _emailService = emailService;
            _signupNotificationService = signupNotificationService;
            _confirmationService = confirmationService;
            _hubContext = hubContext;
        }

        // 🔐 Step 1: Email Entry
        [HttpGet]
        public IActionResult RequestSignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RequestSignUp(RequestConfirmationViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingUser = await _userService.GetUserByEmailAsync(model.Email); // Adjust to your user service
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "An account for this email address already exists.");
                return View(model);
            }
            // Generate confirmation code
            var code = new Random().Next(100000, 999999).ToString();

            // Save code tied to email with expiration logic
            await _confirmationService.SaveCodeAsync(model.Email, code);

            // Send code via email
            await _emailService.SendConfirmationEmailAsync(model.Email, code);

            TempData["Email"] = model.Email;
            return RedirectToAction("VerifyCode");
        }

        // 🔐 Step 2: Code Verification
        [HttpGet]
        public IActionResult VerifyCode()
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("RequestSignUp");

            return View(new ConfirmCodeViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(ConfirmCodeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var isValid = await _confirmationService.VerifyCodeAsync(model.Email, model.Code);
            if (!isValid)
            {
                ModelState.AddModelError("Code", "Invalid or expired confirmation code.");
                return View(model);
            }

            // Save the request
            await _signupNotificationService.SaveSignupRequestAsync(model.Email);

            // Notify admin via SignalR
            await _hubContext.Clients.All.SendAsync("NewSignupRequest", model.Email);

            return RedirectToAction("SignUpRequested", new { email = model.Email });
        }




        [HttpGet]
        public IActionResult SignUpRequested()
        {
            var email = TempData["Email"]?.ToString();
            ViewBag.Email = email;
            return View();
        }
        // Step 1: Request Password Reset
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Use your method here 👇
            var user = await _userService.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "No account found for this email.");
                return View(model);
            }

            // Proceed with generating and sending reset token
            var token = Guid.NewGuid().ToString();
            await _authService.SavePasswordResetTokenAsync(model.Email, token);

            var resetLink = Url.Action("ResetPassword", "Auth", new { token }, Request.Scheme);
            await _emailService.SendPasswordResetEmailAsync(model.Email, resetLink);

            return RedirectToAction("ForgotPasswordConfirmation");
        }


        // Step 2: Confirm Reset Request
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // Step 3: Show Reset Form
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            var valid = await _authService.IsResetTokenValidAsync(token);
            if (!valid) return View("InvalidToken");

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _authService.ResetPasswordAsync(model.Token, model.NewPassword);
            if (!success) return View("InvalidToken");

            return RedirectToAction("Login");
        }



        //[HttpPost]
        //public async Task<IActionResult> CompleteSignUp(RegisterViewModel model)
        //{
        //    if (!ModelState.IsValid) return View(model);

        //    var registerDto = _mapper.Map<RegisterDto>(model);
        //    await _authService.RegisterAsync(registerDto); // Register new user
        //    return RedirectToAction("Login");
        //}

        // 🔐 Existing Login View
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var loginDto = _mapper.Map<LoginDto>(loginViewModel);

            try
            {
                var userDto = await _authService.LoginAsync(loginDto);

                // Handle forced password change
                if ((userDto.RoleId == 1 || userDto.RoleId == 2) && userDto.MustChangePassword)
                {
                    var tempClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                        new Claim("MustChangePassword", "true")
                    };

                    var tempIdentity = new ClaimsIdentity(tempClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(tempIdentity));

                    return RedirectToAction("ChangePassword", "Account");
                }

                // Normal login flow
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                    new Claim(ClaimTypes.Name, userDto.Email),
                    new Claim(ClaimTypes.Role, userDto.RoleId switch
                    {
                        1 => "Professor",
                        2 => "Student",
                        3 => "Admin",
                        _ => "Unknown"
                    })
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var props = new AuthenticationProperties { IsPersistent = false };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), props);

                return userDto.RoleId switch
                {
                    1 => RedirectToAction("ManageQuestions", "Professor"),
                    2 => RedirectToAction("Dashboard", "Student"),
                    3 => RedirectToAction("UserManagement", "Admin"),
                    _ => throw new UnauthorizedAccessException("Invalid role")
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
            var email = User.Identity?.Name;
            if (email != null)
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user != null)
                {
                    await _historyService.LogActionAsync(new HistoryDto
                    {
                        UserId = user.Id,
                        Action = "User Logged Out",
                        IpAddress = "192.1.1.1",
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            Response.Cookies.Delete(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}
