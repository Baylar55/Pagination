using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fiorello.ViewModels;

namespace Fiorello.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel
            {
                HomeMainSlider = await _appDbContext.HomeMainSlider
                                              .Include(h => h.HomeMainSliderPhotos)
                                              .FirstOrDefaultAsync(),
                Products = await _appDbContext.Products
                                        .OrderByDescending(p => p.Id)
                                        .Take(4)
                                        .ToListAsync()
            };
            return View(model);
        }
    }
}
