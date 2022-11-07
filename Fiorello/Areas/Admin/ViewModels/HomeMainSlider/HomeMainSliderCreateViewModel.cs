namespace Fiorello.Areas.Admin.ViewModels.HomeMainSlider
{
    public class HomeMainSliderCreateViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Photo { get; set; }
        public List<IFormFile> SubPhotos { get; set; }
    }
}
