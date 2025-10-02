namespace Ecommerce_Pet_Store.Models
{
    public class ProductType
    {
      
       
        
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int PetTypeId { get; set; }

        public PetType PetType { get; set; }
    }
}
