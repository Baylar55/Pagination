using Fiorello.ViewModels.Product;
using Fiorello.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var model = new ProductIndexViewModel
            {
                Products = await _appDbContext.Products
                                                .OrderByDescending(p => p.Id)
                                                .Take(4)
                                                .ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _appDbContext.Products
                                .Include(p => p.ProductPhotos)
                                .Include(p => p.Category)
                                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                StatusType = product.StatusType,
                Category = product.Category,
                Description = product.Description,
                Quantity = product.Quantity,
                Name = product.Name,
                Dimension = product.Dimension,
                PhotoName = product.PhotoName,
                Weight = product.Weight,
                Cost = product.Cost,
                Photos = product.ProductPhotos,
            };
            return View(model);
        }

        public async Task<IActionResult> LoadMore(int skipRow)
        {
            bool isLast = false;
            var products = await _appDbContext.Products
                                       .OrderByDescending(p => p.Id)
                                       .Skip(4 * skipRow)
                                       .Take(4)
                                       .ToListAsync();
            if (((skipRow + 1) * 4 + 1) > _appDbContext.Products.Count())
            {
                isLast = true;
            }
            
            var model = new ProductLoadMoreViewModel
            {
                Products = products,
                IsLast = isLast
            };

            return PartialView("_ProductPartial", model);
        }

    }
}
