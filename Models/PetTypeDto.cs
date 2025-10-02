using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Pet_Store.Models
{
    public class PetTypeDto
    {
        [Required, MaxLength(100)]
        public string TypeName { get; set; } = "";
    }
}
