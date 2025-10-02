using Ecommerce_Pet_Store.Models;
using EcommercePetStore.Models;

namespace EcommercePetStore.ViewModels
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
