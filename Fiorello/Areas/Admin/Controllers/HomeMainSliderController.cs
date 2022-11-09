using Fiorello.Areas.Admin.ViewModels.HomeMainSlider;
using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Hosting;
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
            foreach (var Photo in model.SubPhotos)
            {
                if (!_fileService.IsImage(Photo))
                {
                    ModelState.AddModelError("Photo", $"{Photo} should be in image format");
                    hasError = true;
                }
                else if (!_fileService.CheckSize(Photo, 400))
                {
                    ModelState.AddModelError("Photo", $"{Photo}'s size should be smaller than 400kb");
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

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var homeMainSlider = await _appDbContext.HomeMainSlider.Include(hs => hs.HomeMainSliderPhotos).FirstOrDefaultAsync(hs => hs.Id == id);
            if (homeMainSlider == null) return NotFound();
            var model = new HomeMainSliderUpdateViewModel
            {
                Title = homeMainSlider.Title,
                Description = homeMainSlider.Description,
                SubPhotoName = homeMainSlider.SubPhotoName,
                homeMainSliderPhotos = homeMainSlider.HomeMainSliderPhotos,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(HomeMainSliderUpdateViewModel model,int id)
        {
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();

            var homeMainSlider = await _appDbContext.HomeMainSlider.Include(hs => hs.HomeMainSliderPhotos).FirstOrDefaultAsync(hs => hs.Id == id);
            model.homeMainSliderPhotos = homeMainSlider.HomeMainSliderPhotos.ToList();
            if (homeMainSlider == null) return NotFound();
            
            if (model.SubPhoto != null)
            {
                if (!_fileService.IsImage(model.SubPhoto))
                {
                    ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                    return View(model);
                }
                if (!_fileService.CheckSize(model.SubPhoto, 400))
                {
                    ModelState.AddModelError("Photo", "Photo size should be less than 400kb");
                    return View(model);
                }
                _fileService.Delete(homeMainSlider.SubPhotoName, _webHostEnvironment.WebRootPath);
                homeMainSlider.SubPhotoName = await _fileService.UploadAsync(model.SubPhoto, _webHostEnvironment.WebRootPath);
            }

            bool hasError = false;

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} should be in image format");
                        hasError = true;
                    }
                    else if (!_fileService.CheckSize(photo, 400))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName}  size should be less than 400kb");
                        hasError = true;
                    }
                }

                if (hasError) { return View(model); }
                var homeMainSliderPhoto = homeMainSlider.HomeMainSliderPhotos.OrderByDescending(hs => hs.Order).FirstOrDefault();
                int order = homeMainSliderPhoto != null ? homeMainSliderPhoto.Order : 0;
                foreach (var photo in model.Photos)
                {
                    var productPhoto = new HomeMainSliderPhoto
                    {
                        Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                        Order = ++order,
                        HomeMainSliderId = homeMainSlider.Id
                    };
                    await _appDbContext.HomeMainSliderPhotos.AddAsync(productPhoto);
                    await _appDbContext.SaveChangesAsync();
                }
            }
            homeMainSlider.Title = model.Title;
            homeMainSlider.Description = model.Description;
            model.SubPhotoName = homeMainSlider.SubPhotoName;
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var homeMainSlider = await _appDbContext.HomeMainSlider
                .Include(sl => sl.HomeMainSliderPhotos).FirstOrDefaultAsync(x => x.Id == id);
            if (homeMainSlider == null) return NotFound();
            var model = new HomeMainSliderDetailsViewModel
            {
                Title = homeMainSlider.Title,
                Description = homeMainSlider.Description,
                Id = homeMainSlider.Id,
                Photos = homeMainSlider.HomeMainSliderPhotos,
                SubPhotoName = homeMainSlider.SubPhotoName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            var homeMainSlider = await _appDbContext.HomeMainSlider.FirstOrDefaultAsync();
            if (homeMainSlider == null) return NotFound();
            _appDbContext.HomeMainSlider.Remove(homeMainSlider);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {
            var homeMainSliderPhoto = await _appDbContext.HomeMainSliderPhotos.FindAsync(id);
            if (homeMainSliderPhoto == null) return NotFound();
            var model = new HomeMainSliderUpdatePhotoViewModel
            {
                Order = homeMainSliderPhoto.Order
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, HomeMainSliderUpdatePhotoViewModel model)
        {

            var homeMainSliderPhoto = await _appDbContext.HomeMainSliderPhotos.FindAsync(id);
            if (homeMainSliderPhoto == null) return NotFound();
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Order", "Order can't be null");
                return View(homeMainSliderPhoto);
            }
            homeMainSliderPhoto.Order = model.Order;
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("details", "homemainslider", new { id = homeMainSliderPhoto.HomeMainSliderId });

        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var homeMainSliderPhoto = await _appDbContext.HomeMainSliderPhotos.FindAsync(id);
            if (homeMainSliderPhoto == null) return NotFound();

            _appDbContext.HomeMainSliderPhotos.Remove(homeMainSliderPhoto);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("details", "homemainslider", new { id = homeMainSliderPhoto.HomeMainSliderId });

        }
    }
}
