namespace Ecommerce_Pet_Store.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = "";

        public List<ProductCategory>? ProductCategories { get; set; }
    }
}
