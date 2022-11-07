using Fiorello.Areas.Admin.ViewModels.Product;
using Fiorello.Areas.Admin.ViewModels.ProductPhoto;
using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public ProductController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        #region Product CRUD

        public async Task<IActionResult> Index()
        {
            var model = new ProductIndexViewModel
            {
                Products = await _appDbContext.Products
                                            .ToListAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProductCreateViewModel
            {
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {

            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToListAsync();


            if (!ModelState.IsValid) return View(model);

            if (await _appDbContext.Products.FindAsync(model.CategoryId) == null)
            {
                ModelState.AddModelError("CategoryId", "This category isn't exist");
            }

            bool isExist = await _appDbContext.Products.AnyAsync(p => p.Name.ToLower().Trim() == model.Name.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "This name is already exist");
            }


            bool hasError = false;
            foreach (var photo in model.ProductPhotos)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", $"{photo.FileName} should be in image format");
                    hasError = true;
                }
                else if (!_fileService.CheckSize(model.Photo, 400))
                {
                    ModelState.AddModelError("Photo", $"{photo.FileName}'s size sould be smaller than 400kb");
                    hasError = true;
                }
            }
            if (hasError) return View(model);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Cost = model.Cost,
                Quantity = model.Quantity,
                Weight = model.Weight,
                CategoryId = model.CategoryId,
                Dimension = model.Dimension,
                StatusType = model.StatusType,
                PhotoName = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.Products.AddAsync(product);
            await _appDbContext.SaveChangesAsync();

            int order = 1;
            foreach (var photo in model.ProductPhotos)
            {
                var productPhoto = new ProductPhoto
                {
                    ProductId = product.Id,
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,

                };
                await _appDbContext.ProductPhotos.AddAsync(productPhoto);
                await _appDbContext.SaveChangesAsync();
                order++;
            }
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var dbProduct = await _appDbContext.Products
                                            .Include(p => p.ProductPhotos)
                                            .FirstOrDefaultAsync(p => p.Id == id);
            if (dbProduct == null) return NotFound();
            var model = new ProductUpdateViewModel
            {
                Name = dbProduct.Name,
                Description = dbProduct.Description,
                Quantity = dbProduct.Quantity,
                Dimension = dbProduct.Dimension,
                StatusType = dbProduct.StatusType,
                CategoryId = dbProduct.CategoryId,
                Weight = dbProduct.Weight,
                PhotoName = dbProduct.PhotoName,
                Cost = dbProduct.Cost,
                ProductPhotos = dbProduct.ProductPhotos,
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                }).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductUpdateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToListAsync();

            if (!ModelState.IsValid) return View(model);

            var dbProduct = await _appDbContext.Products.Include(p => p.ProductPhotos).FirstOrDefaultAsync(p => p.Id == id);
            if (dbProduct == null) return NotFound();

            model.ProductPhotos = dbProduct.ProductPhotos.ToList();

            if (await _appDbContext.Categories.FindAsync(model.CategoryId) == null)
            {
                ModelState.AddModelError("CategoryId", "This category isn't exist");
            }

            bool isExist = await _appDbContext.Products.AnyAsync(p => p.Name.ToLower().Trim() == model.Name.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "This name is already exist");
            }

            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);
            if (category == null) return NotFound();
            if (model.Photo != null)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", $"Uploaded file should be in image format");
                    return View(model);
                }
                else if (!_fileService.CheckSize(model.Photo, 400))
                {
                    ModelState.AddModelError("Photo", "Image's size sould be smaller than 400kb");
                    return View(model);
                }

            }

            dbProduct.Cost = model.Cost;
            dbProduct.Name = model.Name;
            dbProduct.Weight = model.Weight;
            dbProduct.Description = model.Description;
            dbProduct.CategoryId = model.CategoryId;
            dbProduct.Dimension = model.Dimension;
            _fileService.Delete(_webHostEnvironment.WebRootPath, dbProduct.PhotoName);
            dbProduct.PhotoName = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            dbProduct.StatusType = model.StatusType;
            dbProduct.Quantity = model.Quantity;

            bool hasError = false;

            if (model.Photos != null)
            {
                foreach (var photo in model.ProductPhotos)
                {
                    if (!_fileService.IsImage(model.Photo))
                    {
                        ModelState.AddModelError("Photo", $"{photo.Name} should be in image format");
                        hasError = true;
                    }
                    else if (!_fileService.CheckSize(model.Photo, 400))
                    {
                        ModelState.AddModelError("Photo", $"{photo.Name}'s size sould be smaller than 400kb");
                        hasError = true;
                    }

                }
            }
            if (hasError) return View(model);


            int order = 1;
            foreach (var photo in model.Photos)
            {
                foreach (var item in model.ProductPhotos)
                {
                    _fileService.Delete(_webHostEnvironment.WebRootPath, item.Name);
                }
                var productPhoto = new ProductPhoto
                {
                    ProductId = dbProduct.Id,
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,
                };
                await _appDbContext.ProductPhotos.AddAsync(productPhoto);
                await _appDbContext.SaveChangesAsync();
                order++;
            }
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dbProduct = await _appDbContext.Products.Include(p => p.ProductPhotos).FirstOrDefaultAsync(p => p.Id == id);
            if (dbProduct == null) return NotFound();

            _fileService.Delete(_webHostEnvironment.WebRootPath, dbProduct.PhotoName);
            foreach (var item in dbProduct.ProductPhotos)
            {
                _fileService.Delete(_webHostEnvironment.WebRootPath, item.Name);
            }
            _appDbContext.Products.Remove(dbProduct);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var dbProduct = await _appDbContext.Products.Include(p => p.ProductPhotos).FirstOrDefaultAsync(p => p.Id == id);
            if (dbProduct == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                Name = dbProduct.Name,
                Cost = dbProduct.Cost,
                Description = dbProduct.Description,
                Quantity = dbProduct.Quantity,
                Dimension = dbProduct.Dimension,
                Weight = dbProduct.Weight,
                StatusType = dbProduct.StatusType,
                CategoryId = dbProduct.CategoryId,
                PhotoName = dbProduct.PhotoName,
                Photos = dbProduct.ProductPhotos,
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }

        #endregion


        #region ProductPhoto SubCRUD

        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {
            var dbProductPhoto = await _appDbContext.ProductPhotos.FindAsync(id);
            if (dbProductPhoto == null) return NotFound();
            var model = new ProductPhotoUpdateViewModel
            {
                Order = dbProductPhoto.Order,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, ProductPhotoUpdateViewModel model)
        {
            if(!ModelState.IsValid) return View(model);
            if(id!=model.Id) return BadRequest();
            var dbProductPhoto= await _appDbContext.ProductPhotos.FindAsync(id);
            if(dbProductPhoto == null) return NotFound();   
            dbProductPhoto.Order = model.Order;
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("update", "product", new {id=dbProductPhoto.ProductId});
        }

        [HttpGet]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var dbProductPhoto=await _appDbContext.ProductPhotos.FindAsync(id);
            if(dbProductPhoto==null) return NotFound();
            _fileService.Delete(_webHostEnvironment.WebRootPath,dbProductPhoto.Name);
            _appDbContext.ProductPhotos.Remove(dbProductPhoto);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("update");
        }

        #endregion
    }
}
