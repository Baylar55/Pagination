using Fiorello.DAL;
using Fiorello.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Controllers
{
    public class FaqPageController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public FaqPageController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new FaqPageIndexViewModel()
            {
                FaqComponents = await _appDbContext.FaqComponents.ToListAsync()
            };
            return View(model);
        }
    }
}
