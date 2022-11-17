using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static Fiorello.Models.Product;

namespace Fiorello.Areas.Admin.ViewModels.Product
{
    public class ProductIndexViewModel
    {
        public List<Models.Product> Products { get; set; }

        #region Filter

        public string? Name { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public int? CategoryId { get; set; }

        [Display(Name = "Minimum Cost")]
        public double? MinCost { get; set; }

        [Display(Name = "Maximum Cost")]
        public double? MaxCost { get; set; }

        [Display(Name = "Minimum Quantity")]
        public int? MinQuantity { get; set; }

        [Display(Name = "Maximum Quantity")]
        public int? MaxQuantity { get; set; }

        [Display(Name = "Created at start")]
        public DateTime? CreatedAtStart { get; set; }

        [Display(Name = "Created at end")]
        public DateTime? CreatedAtEnd { get; set; }

        public Status? StatusType { get; set; }

        #endregion
    }
}
