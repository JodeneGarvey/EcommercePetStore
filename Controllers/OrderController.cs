using Ecommerce_Pet_Store.Services;
using EcommercePetStore.Models;
using EcommercePetStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommercePetStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List all orders
        public async Task<IActionResult> Index()
        {
            var orders = _context.Orders
            .Select(o => new Order
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                OrderItems = o.OrderItems
            })
            .ToList();

            // Ensure we pass a non-null collection
            return View(orders);
        }

        // Order details
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // Change order status (e.g., Pending → Shipped → Delivered)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                TempData["CancelError"] = "Order not found.";
                return RedirectToAction("MyOrders", "Cart");
            }

            if (order.Status == "Shipped" || order.Status == "Delivered" || order.Status == "Cancelled")
            {
                TempData["CancelError"] = "This order cannot be cancelled.";
            }
            else
            {
                order.Status = "Cancelled";

                // (Optional) Restore stock
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity;
                        _context.Update(product);
                    }
                }

                _context.Update(order);
                await _context.SaveChangesAsync();

                TempData["CancelSuccess"] = "Your order has been cancelled successfully.";
            }

            // ✅ Redirect back to order confirmation
            return RedirectToAction("OrderConfirmation", "Cart", new { id = order.Id });
        }
    }
}
