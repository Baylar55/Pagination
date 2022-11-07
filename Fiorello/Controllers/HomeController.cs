using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
