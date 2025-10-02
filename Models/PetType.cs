namespace Ecommerce_Pet_Store.Models
{
    public class PetType
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public List<ProductType>? ProductTypes { get; set; }
    }
}
