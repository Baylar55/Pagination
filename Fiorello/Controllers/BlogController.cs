using Fiorello.ViewModels.Blog;
using Fiorello.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fiorello.Models;

namespace Fiorello.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public BlogController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index(BlogIndexViewModel model)
        {

            model = new BlogIndexViewModel
            {
                Blogs = await PaginateBlogsAsync(model.Page, model.Take),
                PageCount =await GetPageCountAsync(model.Take),
                Page=model.Page
            };
            return View(model);
        }

        private async Task<List<Blog>> PaginateBlogsAsync(int page, int take)
        {
            return await _appDbContext.Blogs
                                    .OrderByDescending(b => b.Id)
                                    .Skip((page-1)*take)
                                    .Take(take)
                                    .ToListAsync();
        }

        private async Task<int> GetPageCountAsync(int take)
        {
            var blogCount = _appDbContext.Blogs.Count();
            return (int)Math.Ceiling((decimal)blogCount / take);
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _appDbContext.Blogs.FindAsync(id);
            if (blog == null) return NotFound();

            var model = new BlogDetailsViewModel
            {
                Title = blog.Title,
                CreateDate = blog.CreateDate,
                Description = blog.Description,
                PhotoName = blog.PhotoName,
            };
            return View(model);
        }
    }
}
