using Fiorello.Models;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
                
        }
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Product> Products { get; set; } 
        public DbSet<ProductPhoto> ProductPhotos { get; set; } 
        public DbSet<Expert> Experts { get; set; }
        public DbSet<FaqComponent> FaqComponents { get; set; }
        public DbSet<HomeMainSlider> HomeMainSlider { get; set; }
        public DbSet<HomeMainSliderPhoto> HomeMainSliderPhotos { get; set; }
        public DbSet<Blog> Blogs { get; set; }
    }
}
