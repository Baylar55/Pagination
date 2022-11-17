namespace Fiorello.Models
{
    public class Basket
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public ICollection<BasketProduct> BasketProducts { get; set; }
    }
}
