using Fiorello.Areas.Admin.ViewModels.FaqComponent;
using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FaqComponentController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public FaqComponentController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new FaqComponentIndexViewModel
            {
                FaqComponents = await _appDbContext.FaqComponents.ToListAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FaqComponentCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var faqComponent = new FaqComponent
            {
                Title = model.Title,
                Description = model.Description,
                Order = model.Order
            };
            await _appDbContext.FaqComponents.AddAsync(faqComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var faqComponent = await _appDbContext.FaqComponents.FindAsync(id);
            if (faqComponent == null) return NotFound();
            var model = new FaqComponentUpdateViewModel
            {
                Title = faqComponent.Title,
                Description = faqComponent.Description,
                Order = faqComponent.Order
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, FaqComponentUpdateViewModel model)
        {
            var faqComponent = await _appDbContext.FaqComponents.FindAsync(id);
            if (faqComponent == null) return NotFound();

            faqComponent.Title = model.Title;
            faqComponent.Description = model.Description;
            faqComponent.Order = model.Order;
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var faqComponent = await _appDbContext.FaqComponents.FindAsync(id);
            if (faqComponent == null) return NotFound();

             _appDbContext.Remove(faqComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var faqComponent = await _appDbContext.FaqComponents.FindAsync(id);
            if (faqComponent == null) return NotFound();
            var model = new FaqComponentDetailsViewModel
            {
                Title = faqComponent.Title,
                Description = faqComponent.Description,
                Order = faqComponent.Order
            };
            return View(model);
        }
    }
}
