using Fiorello.DAL;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Controllers
{
    [Authorize]
    public class AboutController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public AboutController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
