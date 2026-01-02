using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Extensions;
using PresentationLayer.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        if (!User.HasClaim(c => c.Type == "MustChangePassword" && c.Value == "true"))
        {
            return RedirectToAction("Login", "Auth"); // If unauthorized, send back to login
        }

        return View();
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get user ID from claims
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Auth"); // If no ID, force login
        }

        var user = await _userService.GetUserByIdAsync(int.Parse(userId));

        if (user == null)
        {
            return NotFound();
        }

        var success = await _userService.ChangePasswordAsync(user.Id, model.NewPassword);

        if (!success)
        {
            return StatusCode(500, "Error updating password");
        }

        // ✅ Remove temporary authentication
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // ✅ Fully authenticate user
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.RoleId switch
        {
            1 => "Professor",
            2 => "Student",
            3 => "Admin",
            _ => "Unknown"
        })
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        TempData["SuccessMessage"] = "Password changed successfully.";

        // ✅ Redirect based on user role
        return user.RoleId switch
        {
            1 => RedirectToAction("ManageCourses", "Professor"),
            2 => RedirectToAction("Dashboard", "Student"),
            3 => RedirectToAction("UserManagement", "Admin"),
            _ => RedirectToAction("Login", "Auth") // Default case
        };
    }
    [Authorize]
    [HttpGet]
    public IActionResult ProfileChangePassword()
    {
        return View();
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ProfileChangePassword(ProfileChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Return the form with validation errors
        }

        // Get the currently logged-in user's ID
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Get ID from JWT or Cookie

        // Call ChangePasswordAsync in UserService
        var result = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "The current password is incorrect or an error occurred.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Your password has been changed successfully.";
        return RedirectToAction("ManageQuestions", "Professor");
    }


}
