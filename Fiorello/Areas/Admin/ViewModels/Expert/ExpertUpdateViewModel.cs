namespace Fiorello.Areas.Admin.ViewModels.Expert
{
    public class ExpertUpdateViewModel
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string PhotoName { get; set; }
        public IFormFile Photo { get; set; }
    }
}
