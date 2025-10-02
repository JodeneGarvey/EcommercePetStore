namespace Ecommerce_Pet_Store.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public List<ProductCategory>? ProductCategories { get; set; }

        public List<ProductType>? ProductTypes { get; set; }
    }
}
