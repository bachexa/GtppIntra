using Microsoft.AspNetCore.Mvc;

namespace Envirovement.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View("/Views/Shared/_Login.cshtml");
        }
    }
}
