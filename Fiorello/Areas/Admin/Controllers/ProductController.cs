using Fiorello.Areas.Admin.ViewModels.Product;
using Fiorello.Areas.Admin.ViewModels.ProductPhoto;
using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Fiorello.Models.Product;

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



        public async Task<IActionResult> Index(ProductIndexViewModel model)
        {
            var products = FilterProducts(model);
            model = new ProductIndexViewModel
            {
                Products = await products.Include(p => p.Category).ToListAsync(),
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .ToListAsync()
            };
            return View(model);

        }

        #region Filter Methods

        private IQueryable<Product> FilterProducts(ProductIndexViewModel model)
        {
            var products = FilterByName(model.Name);
            products = FilterByCategory(products, model.CategoryId);
            products = FilterByCost(products, model.MinCost, model.MaxCost);
            products = FilterByQuantity(products, model.MinQuantity,model.MaxQuantity);
            products = FilterByCreatedAt(products, model.CreatedAtStart,model.CreatedAtEnd);
            products = FilterByStatus(products, model.StatusType);
            return products;
        }


        private IQueryable<Product> FilterByName(string name)
        {
            return _appDbContext.Products.Where(p => !string.IsNullOrEmpty(name) ? p.Name.Contains(name) : true);
        }

        private IQueryable<Product> FilterByCategory(IQueryable<Product> products, int? categoryId)
        {
            return _appDbContext.Products.Where(p => categoryId != null ? p.CategoryId == categoryId : true);
        }

        private IQueryable<Product> FilterByCost(IQueryable<Product> product, double? minCost, double? maxCost)
        {
            return _appDbContext.Products.Where(p => (minCost != null ? p.Cost >= minCost : true) && (maxCost != null ? p.Cost <= maxCost : true));
        }

        private IQueryable<Product> FilterByQuantity(IQueryable<Product> product, int? minQuantity, int? maxQuantity)
        {
            return _appDbContext.Products.Where(p => (minQuantity != null ? p.Quantity >= minQuantity : true) && (maxQuantity != null ? p.Quantity <= maxQuantity : true));
        }

        private IQueryable<Product> FilterByCreatedAt(IQueryable<Product> product, DateTime? createdAtStart, DateTime? createdAtEnd)
        {
            return _appDbContext.Products.Where(p => (createdAtStart != null ? p.CreatedAt >= createdAtStart : true) && (createdAtEnd != null ? p.CreatedAt <=createdAtEnd : true));
        }

        private IQueryable<Product> FilterByStatus(IQueryable<Product> product, Status? statusType)
        {
            return _appDbContext.Products.Where(p => statusType != null ? p.StatusType==statusType : true);
        }

        #endregion

        #region Product CRUD
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
            model.Categories = await _appDbContext.Categories
               .Select(pt => new SelectListItem
               {
                   Text = pt.Name,
                   Value = pt.Id.ToString()

               }).ToListAsync();

            var product = await _appDbContext.Products
                         .Include(p => p.ProductPhotos)
                         .FirstOrDefaultAsync(x => x.Id == id);

            bool isExists = await _appDbContext.Products.AnyAsync(p => p.Name.ToLower().Trim() == model.Name.ToLower().Trim()
            && model.Id != p.Id);

            if (isExists)
            {
                ModelState.AddModelError("Title", "This product already have");
                return View(model);
            }


            var dbproduct = await _appDbContext.Products.FindAsync(id);
            if (dbproduct == null) return NotFound();
            if (model.Id != dbproduct.Id) return BadRequest();


            bool hasError = false;
            int maxSize = 1000;

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        ModelState.AddModelError("Photos", "File must be image");
                        hasError = true;
                    }
                    else if (!_fileService.CheckSize(photo, maxSize))
                    {
                        ModelState.AddModelError("Photos", $"Photo size must be less {maxSize}kb");
                        hasError = true;
                    }



                    int order = 1;
                    var productPhoto = new ProductPhoto
                    {
                        Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                        Order = order,
                        ProductId = product.Id
                    };
                    await _appDbContext.ProductPhotos.AddAsync(productPhoto);
                    await _appDbContext.SaveChangesAsync();
                    order++;

                }
                if (hasError)
                {
                    return View(model);
                }

            }

            if (model.Photo != null)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photos", "File must be image");
                    hasError = true;
                }
                else if (!_fileService.CheckSize(model.Photo, maxSize))
                {
                    ModelState.AddModelError("Photos", $"Photo size must be less {maxSize}kb");
                    hasError = true;
                }
                dbproduct.PhotoName = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
                await _appDbContext.SaveChangesAsync();
            }
            if (hasError)
            {
                return View(model);
            }

            if (!ModelState.IsValid) return View(model);
            dbproduct.Name = model.Name;
            dbproduct.Description = model.Description;
            dbproduct.StatusType = model.StatusType;
            dbproduct.CategoryId = model.CategoryId;
            dbproduct.Cost = model.Cost;
            dbproduct.Quantity = model.Quantity;
            dbproduct.Weight = model.Weight;

            await _appDbContext.SaveChangesAsync();

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
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();
            var dbProductPhoto = await _appDbContext.ProductPhotos.FindAsync(id);
            if (dbProductPhoto == null) return NotFound();
            dbProductPhoto.Order = model.Order;
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("update", "product", new { id = dbProductPhoto.ProductId });
        }

        [HttpGet]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var dbProductPhoto = await _appDbContext.ProductPhotos.FindAsync(id);
            if (dbProductPhoto == null) return NotFound();
            _fileService.Delete(_webHostEnvironment.WebRootPath, dbProductPhoto.Name);
            _appDbContext.ProductPhotos.Remove(dbProductPhoto);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("update");
        }

        #endregion
    }
}
