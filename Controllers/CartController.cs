using Ecommerce_Pet_Store.Models;
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
    public class CartController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private const string CartSessionKey = "CartSession";
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.environment = environment;
            this._userManager = userManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            string? userId = _userManager.GetUserId(User);

            if (User.Identity != null && User.Identity.IsAuthenticated && !string.IsNullOrEmpty(userId))
            {
                // Logged-in user: get cart from DB
                var userCart = context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToList();

                return View(userCart);
            }
            else
            {
                // Guest: get cart from session
                var cart = HttpContext.Session.GetObject<List<CartItems>>(CartSessionKey) ?? new List<CartItems>();
                return View(cart);
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (quantity < 1)
            {
                TempData["Error"] = "Invalid quantity.";
                return RedirectToAction("Index", "Product");
            }

            var product = context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index", "Product");
            }

            if (product.Quantity < quantity)
            {
                TempData["Error"] = "Not enough stock.";
                return RedirectToAction("Index", "Product");
            }

            product.Quantity -= quantity;
            context.SaveChanges();

            string? userId = _userManager.GetUserId(User);

            if (User.Identity != null && User.Identity.IsAuthenticated && !string.IsNullOrEmpty(userId))
            {
                // Logged-in user: Save to database
                var existingItem = context.CartItems
                    .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    context.CartItems.Add(new CartItems
                    {
                        UserId = userId,
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        ImageUrl = product.ImageUrl ?? "",
                        Price = product.Price,
                        Quantity = quantity
                    });
                }

                await context.SaveChangesAsync();
            }
            else
            {
                // Guest: Use session
                var cart = HttpContext.Session.GetObject<List<CartItems>>(CartSessionKey) ?? new List<CartItems>();

                var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItems
                    {
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        ImageUrl = product.ImageUrl ?? "",
                        Price = product.Price,
                        Quantity = quantity
                    });
                }

                HttpContext.Session.SetObject(CartSessionKey, cart);
            }

            TempData["Success"] = "Product added to cart.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            string? userId = _userManager.GetUserId(User);

            if (User.Identity != null && User.Identity.IsAuthenticated && !string.IsNullOrEmpty(userId))
            {
                var item = context.CartItems
                    .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

                if (item != null)
                {
                    context.CartItems.Remove(item);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                var cart = HttpContext.Session.GetObject<List<CartItems>>(CartSessionKey) ?? new List<CartItems>();
                var item = cart.FirstOrDefault(c => c.ProductId == productId);
                if (item != null)
                {
                    cart.Remove(item);
                    HttpContext.Session.SetObject(CartSessionKey, cart);
                }
            }

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult Checkout()
        {
            return View(new CheckoutViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.PaymentMethod != "Cash")
            {
                if (string.IsNullOrEmpty(model.CardNumber) ||
                    string.IsNullOrEmpty(model.ExpiryDate) ||
                    string.IsNullOrEmpty(model.CVV))
                {
                    ModelState.AddModelError("", "Card details are required for Credit/Debit payments.");
                    return View(model);
                }
            }

            var userId = _userManager.GetUserId(User);
            var cartItems = context.CartItems.Where(c => c.UserId == userId).ToList();

            var order = new Order
            {
                UserId = userId,
                CustomerName = model.CustomerName,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                PaymentMethod = model.PaymentMethod,
                OrderDate = DateTime.Now,
                Status = "Pending",
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList()
            };

            context.Orders.Add(order);
            context.CartItems.RemoveRange(cartItems);
            context.SaveChanges();

            return RedirectToAction("OrderConfirmation", new { id = order.Id });
        }

        [HttpGet]
        
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            string? userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        
    }
}

    


        
    


