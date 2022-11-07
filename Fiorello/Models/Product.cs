using System.ComponentModel.DataAnnotations;

namespace Fiorello.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoName { get; set; }
        public double Cost { get; set; }
        public string Description  { get; set; }
        public int Quantity { get; set; }
        public string Weight { get; set; }
        public string Dimension { get; set; }
        public ICollection<ProductPhoto> ProductPhotos { get; set; }
        public Category Category { get; set; }
        public int? CategoryId { get; set; }
        public Status StatusType { get; set; }
        public enum Status
        {
            New,
            Sold,
            Sale,
        };
    }
}
