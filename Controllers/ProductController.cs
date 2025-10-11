using Ecommerce_Pet_Store.Models;
using Ecommerce_Pet_Store.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Pet_Store.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        

        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("SearchResults", new List<Product>());
            }

            // Search by product name or any category name linked to that product
            var results = await context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Where(p => p.ProductName.Contains(query) ||
                            p.ProductCategories.Any(pc => pc.Category.CategoryName.Contains(query)))
                .ToListAsync();

            return View("SearchResults", results);
        }
    
    [Authorize]
        public IActionResult Index()
        {
            var products = context.Products
                .Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
                .Include(p => p.ProductTypes).ThenInclude(pt => pt.PetType)
                .ToList();
            return View(products);
        }
        [Authorize]
        public IActionResult Create()
        {
            
            
            var option = new ProductDto
            {
                AvailableCategories = context.Categories.ToList(),
                AvailableTypes = context.PetTypes.ToList(),
            };
            return View(option);
            
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                productDto.AvailableCategories = context.Categories.ToList();
                productDto.AvailableTypes = context.PetTypes.ToList();
                return View(productDto);
            }
            


            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/Products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            var product = new Product
            {
                ProductName = productDto.Name,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                ImageUrl = newFileName,
                ProductCategories = new List<ProductCategory>(),
                ProductTypes = new List<ProductType>()
            };

            // Add related categories
            foreach (var categoryId in productDto.SelectedCategoryIds)
            {
                product.ProductCategories.Add(new ProductCategory
                {
                    CategoryId = categoryId
                });
            }

            // Add related types
            foreach (var typeId in productDto.SelectedTypeIds)
            {
                product.ProductTypes.Add(new ProductType
                {
                    PetTypeId = typeId
                });
            }

            // Add product and its relations in a single SaveChanges()
            context.Products.Add(product);
            var affected = context.SaveChanges(); // ✅ This should now insert everything

            Console.WriteLine($"Rows affected: {affected}");

            return RedirectToAction("Index", "Product");
        
            

        }
        [Authorize]
        public IActionResult Edit(int id)
        {
            var product = context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductTypes)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            var dto = new ProductDto
            {
                Name = product.ProductName,
                Price = product.Price,
                Quantity = product.Quantity,
                SelectedCategoryIds = product.ProductCategories.Select(pc => pc.CategoryId).ToList(),
                SelectedTypeIds = product.ProductTypes.Select(pt => pt.PetTypeId).ToList(),
                AvailableCategories = context.Categories.ToList(),
                AvailableTypes = context.PetTypes.ToList(),
                ExistingImageUrl = product.ImageUrl
            };

            return View(dto);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Edit(int id, ProductDto dto)
        {
            var product = context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductTypes)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                dto.AvailableCategories = context.Categories.ToList();
                dto.AvailableTypes = context.PetTypes.ToList();
                return View(dto);
            }

            product.ProductName = dto.Name;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;

            if (dto.ImageFile != null)
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(dto.ImageFile.FileName);
                string fullPath = Path.Combine(environment.WebRootPath, "Products", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    dto.ImageFile.CopyTo(stream);
                }

                // Optionally delete old image
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldPath = Path.Combine(environment.WebRootPath, "Products", product.ImageUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                product.ImageUrl = fileName;
            }

            // Clear and re-add categories
            context.ProductCategories.RemoveRange(product.ProductCategories);
            foreach (var categoryId in dto.SelectedCategoryIds)
            {
                context.ProductCategories.Add(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = categoryId
                });
            }

            // Clear and re-add types
            context.ProductTypes.RemoveRange(product.ProductTypes);
            foreach (var typeId in dto.SelectedTypeIds)
            {
                context.ProductTypes.Add(new ProductType
                {
                    ProductId = product.Id,
                    PetTypeId = typeId
                });
            }

            context.SaveChanges();

            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Delete(int id)
        {
            var product = context.Products
                .Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
                .Include(p => p.ProductTypes).ThenInclude(pt => pt.PetType)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductTypes)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string path = Path.Combine(environment.WebRootPath, "Products", product.ImageUrl);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }


            context.ProductCategories.RemoveRange(product.ProductCategories);
            context.ProductTypes.RemoveRange(product.ProductTypes);
            context.Products.Remove(product);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public async Task<IActionResult> ByCategory(int id, string? subcategory)
        {
            var query = context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductTypes)
                .Where(p => p.ProductCategories.Any(pc => pc.CategoryId == id));

            if (!string.IsNullOrEmpty(subcategory))
            {
                query = query.Where(p => p.ProductTypes.Any(pt => pt.PetType.Name == subcategory));
            }

            var products = await query.ToListAsync();
            return View(products);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProductDetails(int? id)
        {
           
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);

        }
        [AllowAnonymous]
        public async Task<IActionResult> ProductsCategories()
        {
            var categoriesWithProducts = await context.Categories
                .Include(c => c.ProductCategories!)
                .ThenInclude(pc => pc.Product).ToListAsync();
            return View(categoriesWithProducts);
        }
    }
}
