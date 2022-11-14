using Fiorello.Areas.Admin.ViewModels.Category;
using Fiorello.DAL;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public CategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new CategoryIndexViewModel
            {
                Categories = await _appDbContext.Categories.ToListAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            bool isExist = await _appDbContext.Categories.AnyAsync(c => c.Name.ToLower().Trim() == model.Name.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "This name is already exist");
                return View(model);
            }
            var category = new Category
            {
                Name = model.Name,
            };
            await _appDbContext.AddAsync(category);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var dbCategory = await _appDbContext.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            var model = new CategoryUpdateViewModel
            {
                Name = dbCategory.Name,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CategoryUpdateViewModel model, int id)
        {
            if (!ModelState.IsValid) return View();
            var dbCategory = await _appDbContext.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            bool isExist = await _appDbContext.Categories.AnyAsync(c => c.Name.ToLower().Trim() == model.Name.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "This name is already exist");
                return View(model);
            }
            if (id != model.Id) return BadRequest();
            dbCategory.Name = model.Name;
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dbCategory = await _appDbContext.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            _appDbContext.Remove(dbCategory);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var dbCategory = await _appDbContext.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            var model = new CategoryDetailsViewModel
            {
                Id = dbCategory.Id,
                Name = dbCategory.Name,
            };
            return View(model);
        }
    }
}
