using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Ecommerce_Pet_Store.Models;
namespace Ecommerce_Pet_Store.Models
{
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";
        [Required]
        public decimal Price { get; set; }

        public IFormFile? ImageFile { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please Enter Qunatity of Products Available")]
        [Display(Name = "Quantity")]
        [Range(1, 500, ErrorMessage = "Available Quantity can range from 1 to 500")]
        public int Quantity { get; set; }
        public string? ExistingImageUrl { get; set; }
        [Required(ErrorMessage = "Please select at least one type.")]
        public List<int> SelectedTypeIds { get; set; } = new();
        [Required(ErrorMessage = "Please select at least one category.")]
        public List<int> SelectedCategoryIds { get; set; } = new();

        public List<PetType> AvailableTypes { get; set; } = new();
        public List<Category> AvailableCategories { get; set; } = new();
    }
}
