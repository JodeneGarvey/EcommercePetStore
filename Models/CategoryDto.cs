using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Pet_Store.Models
{
    public class CategoryDto
    {
        [Required, MaxLength(100)]
        public string CategoryName { get; set; } = "";
    }
}
