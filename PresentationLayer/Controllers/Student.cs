using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Authorize(Policy = "StudentPolicy")]
    public class StudentController : Controller
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        
    }
}
