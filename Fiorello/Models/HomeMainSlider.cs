namespace Fiorello.Models
{
    public class HomeMainSlider
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SubPhotoName { get; set; }
        public ICollection<HomeMainSliderPhoto> HomeMainSliderPhotos { get; set; }
    }
}
