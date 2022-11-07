using Fiorello.Areas.Admin.ViewModels.HomeMainSlider;
using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeMainSliderController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public HomeMainSliderController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeMainSliderIndexViewModel
            {
                HomeMainSlider = await _appDbContext.HomeMainSlider.FirstOrDefaultAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HomeMainSliderCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "Photo should be in image format");
                return View(model);
            }
            else if (!_fileService.CheckSize(model.Photo, 400))
            {
                ModelState.AddModelError("Photo", $"Photo's size sould be smaller than 400kb");
                return View(model);
            }



            bool hasError = false;
            foreach (var subPhoto in model.SubPhotos)
            {
                if (!_fileService.IsImage(subPhoto))
                {
                    ModelState.AddModelError("Photo", $"{subPhoto} should be in image format");
                    hasError = true;
                }
                else if (!_fileService.CheckSize(subPhoto, 400))
                {
                    ModelState.AddModelError("Photo", $"{subPhoto}'s size sould be smaller than 400kb");
                    hasError = true;
                }
            }

            if (hasError) return View(model);

            var homeMainSlider = new HomeMainSlider
            {
                Title = model.Title,
                Description = model.Description,
                SubPhotoName = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };
            await _appDbContext.HomeMainSlider.AddAsync(homeMainSlider);
            await _appDbContext.SaveChangesAsync();

            int order = 1;
            foreach (var photo in model.SubPhotos)
            {
                var homeMainSliderPhoto = new HomeMainSliderPhoto
                {
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,
                    HomeMainSliderId = homeMainSlider.Id,
                };
                await _appDbContext.AddAsync(homeMainSliderPhoto);
                await _appDbContext.SaveChangesAsync();
                order++;
            }
            return RedirectToAction("Index");
        }
    }
}
