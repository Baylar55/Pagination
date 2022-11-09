using Fiorello.Models;
using System.ComponentModel.DataAnnotations;

namespace Fiorello.Areas.Admin.ViewModels.HomeMainSlider
{
    public class HomeMainSliderDetailsViewModel
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string SubPhotoName { get; set; }
        public ICollection<Models.HomeMainSliderPhoto> Photos { get; set; }
    }
}
