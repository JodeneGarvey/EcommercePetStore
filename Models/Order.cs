using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EcommercePetStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required] 
        public string CustomerName { get; set; }
        [Required] 
        public string Address { get; set; }
        [Required, Phone] 
        public string Phone { get; set; }
        [EmailAddress] 
        public string Email { get; set; }

       

        public string? UserId { get; set; }   // logged-in user tracking

        public IdentityUser User { get; set; }

        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public string Status { get; set; } = "Pending";
    }
}
