namespace Fiorello.Models
{
    public class HomeMainSliderPhoto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int HomeMainSliderId { get; set; }
        public HomeMainSlider HomeMainSlider { get; set; }
    }
}
