using Microsoft.AspNetCore.Mvc.Rendering;
using static Fiorello.Models.Product;

namespace Fiorello.Areas.Admin.ViewModels.Product
{
    public class ProductCreateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? PhotoName { get; set; }
        public IFormFile Photo { get; set; }
        public double Cost { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Weight { get; set; }
        public string Dimension { get; set; }
        public List<IFormFile> ProductPhotos { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public int? CategoryId { get; set; }
        public Status StatusType { get; set; }
    }
}
