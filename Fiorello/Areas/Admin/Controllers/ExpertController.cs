using Fiorello.Areas.Admin.ViewModels.Expert;
using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public ExpertController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ExpertIndexViewModel
            {
                Experts = await _appDbContext.Experts.ToListAsync()
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpertCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(model);
            }

            else if (!_fileService.CheckSize(model.Photo, 300))
            {
                ModelState.AddModelError("Photo", "Image size should be smaller than 300 kB");
                return View(model);
            }

            var expert = new Expert
            {
                Name = model.Name,
                Position = model.Position,
                PhotoName=await _fileService.UploadAsync(model.Photo,_webHostEnvironment.WebRootPath)
            };

            await _appDbContext.Experts.AddAsync(expert);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var dbExpert = await _appDbContext.Experts.FindAsync(id);
            if (dbExpert == null) return NotFound();

            var model = new ExpertUpdateViewModel
            {
                Name=dbExpert.Name,
                PhotoName=dbExpert.PhotoName,
                Position=dbExpert.Position
            };
            return View(model);
        }

        [HttpPost] 
        public async Task<IActionResult> Update(int id,ExpertUpdateViewModel model)
        {
            var dbExpert = await _appDbContext.Experts.FindAsync(id);
            if (dbExpert == null) return NotFound();

            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(model);
            }

            else if (!_fileService.CheckSize(model.Photo, 300))
            {
                ModelState.AddModelError("Photo", "Image size should be smaller than 300 kB");
                return View(model);
            }

            dbExpert.Name=model.Name;
            dbExpert.Position = model.Position;
            _fileService.Delete(_webHostEnvironment.WebRootPath, dbExpert.PhotoName);
            dbExpert.PhotoName = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dbExpert = await _appDbContext.Experts.FindAsync(id);
            if (dbExpert == null) return NotFound();

            _fileService.Delete(_webHostEnvironment.WebRootPath, dbExpert.PhotoName);
            _appDbContext.Experts.Remove(dbExpert);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var dbExpert = await _appDbContext.Experts.FindAsync(id);
            if (dbExpert == null) return NotFound();

            var model = new ExpertDetailsViewModel
            {
                Name = dbExpert.Name,
                PhotoName = dbExpert.PhotoName,
                Position = dbExpert.Position
            };
            return View(model);
        }
    }
}
