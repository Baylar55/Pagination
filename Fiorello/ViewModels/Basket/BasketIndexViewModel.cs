using Fiorello.ViewModels.BasketProduct;

namespace Fiorello.ViewModels.Basket
{
    public class BasketIndexViewModel
    {
        public BasketIndexViewModel()
        {
            BasketProducts=new List<BasketProductViewModel>();
        }
        public List<BasketProductViewModel> BasketProducts { get; set; }
    }
}
