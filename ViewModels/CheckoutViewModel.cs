using System.ComponentModel.DataAnnotations;

namespace EcommercePetStore.ViewModels
{
    public class CheckoutViewModel
    {
        [Required] public string CustomerName { get; set; }
        [Required] public string Address { get; set; }
        [Required, Phone] public string Phone { get; set; }
        [EmailAddress] public string Email { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        // ✅ Card details (optional for Cash)
        public string? CardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
    }
}
