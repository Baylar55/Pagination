using Fiorello.Models;
using static Fiorello.Models.Product;
namespace Fiorello.ViewModels.Product
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Weight { get; set; }
        public string Dimension { get; set; }
        public Category Category { get; set; }
        public Status StatusType { get; set; }
        public string PhotoName { get; set; }
        public ICollection<Models.ProductPhoto> Photos { get; set; }
    }
}
