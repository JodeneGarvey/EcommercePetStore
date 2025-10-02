using Ecommerce_Pet_Store.Models;
using Ecommerce_Pet_Store.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Pet_Store.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        [Authorize]
        public IActionResult Index()
        {
            var categories = context.Categories.ToList();
            return View(categories);
            
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult Create(CategoryDto categorydto)
        {
            Category category = new Category();
            {
                category.CategoryName = categorydto.CategoryName;
            };

            context.Categories.Add(category);
            context.SaveChanges();

            return RedirectToAction("Index", "Category");
        }
        [Authorize]
        public IActionResult Edit(int id, Category category)
        {
            using (var db =  context)
            {
                var tempcat = db.Categories.Where(c => c.Id == category.Id).FirstOrDefault();
                TempData["tempcat"] = tempcat;

            }
            return View();
        }
            
        [HttpPost]
        [Authorize]
        public IActionResult Edit(Category category)
        {
            
            
            var UpdateCategory = context.Categories.Where(c => c.Id == category.Id).FirstOrDefault();

            UpdateCategory.CategoryName = category.CategoryName;

            context.SaveChanges();
            
            return RedirectToAction("Index", "Category");
        }
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cat = await context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cat == null)
            {
                return NotFound();
            }

            return View(cat);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cat = await context.Categories.FindAsync(id);
            if (cat != null)
            {
                context.Categories.Remove(cat);
            }
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CatExists(int id)
        {
            return context.Categories.Any(e => e.Id == id);
        }
    }
}
