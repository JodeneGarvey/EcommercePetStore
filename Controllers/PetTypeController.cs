using Ecommerce_Pet_Store.Models;
using Ecommerce_Pet_Store.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Pet_Store.Controllers
{
    public class PetTypeController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public PetTypeController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        [Authorize]
        public IActionResult Index()
        {
            var types = context.PetTypes.ToList();
            return View(types);
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult Create(PetTypeDto typedto)
        {
            PetType type = new PetType();
            {
                type.Name = typedto.TypeName;
            };

            context.PetTypes.Add(type);
            context.SaveChanges();

            return RedirectToAction("Index", "PetType");
        }
        [Authorize]
        public IActionResult Edit(int id, PetType type)
        {
            using (var db = context)
            {
                var temptype = db.PetTypes.Where(t => t.Id == type.Id).FirstOrDefault();
                TempData["temptype"] = temptype;

            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(PetType type)
        {


            var UpdateType = context.PetTypes.Where(t => t.Id == type.Id).FirstOrDefault();

            UpdateType.Name = type.Name;

            context.SaveChanges();

            return RedirectToAction("Index", "PetType");
        }
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var type = await context.PetTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (type == null)
            {
                return NotFound();
            }

            return View(type);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var type = await context.PetTypes.FindAsync(id);
            if (type != null)
            {
                context.PetTypes.Remove(type);
            }
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeExists(int id)
        {
            return context.PetTypes.Any(e => e.Id == id);
        }
    }
}
