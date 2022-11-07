using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fiorello.Models
{
    public class Expert
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Position { get; set; }
        public string? PhotoName { get; set; }

        [NotMapped]
        [Required]
        public IFormFile Photo { get; set; }
    }
}
