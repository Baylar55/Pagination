using Fiorello.Models;

namespace Fiorello.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Models.Product> Products { get; set; }
        public HomeMainSlider? HomeMainSlider { get; set; }
    }
}
