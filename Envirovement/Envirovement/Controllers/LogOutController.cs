using Microsoft.AspNetCore.Mvc;

namespace Envirovement.Controllers
{
    public class LogOutController : Controller
    {
        [HttpGet]
        [Route("login")]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;  // Pass the returnUrl to the view
            return View();
        }
    }
}
