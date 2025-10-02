using Microsoft.EntityFrameworkCore;
using Ecommerce_Pet_Store.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EcommercePetStore.Models;

namespace Ecommerce_Pet_Store.Services
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>()
        .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);

            // ProductType one-to-many (Product ↔ ProductType, PetType ↔ ProductType)
            modelBuilder.Entity<ProductType>()
                .HasKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductType>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTypes)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductType>()
                .HasOne(pt => pt.PetType)
                .WithMany(ptype => ptype.ProductTypes)
                .HasForeignKey(pt => pt.PetTypeId);

            base.OnModelCreating(modelBuilder);
        }
    
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<PetType> PetTypes { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<CartItems> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }


    }
}
