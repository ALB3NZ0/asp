using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoes_store.Models;
using System.Linq;
using System.Security.Claims;

public class CatalogController : Controller
{
    private readonly ShoppingDbContext _context;

    public CatalogController(ShoppingDbContext context)
    {
        _context = context;
    }

    
    public IActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder)
    {
        ViewBag.CurrentFilter = searchTerm;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;

        var products = _context.Products.AsQueryable();

        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            products = products.Where(p => p.Name.Contains(searchTerm) ||
                                            p.Description.Contains(searchTerm));
        }

        if (minPrice.HasValue)
        {
            products = products.Where(p => p.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= maxPrice.Value);
        }

        ViewBag.PriceSortParamAsc = sortOrder == "PriceAsc" ? "price_desc" : "PriceAsc";
        ViewBag.PriceSortParamDesc = sortOrder == "PriceDesc" ? "price_asc" : "PriceDesc";

        switch (sortOrder)
        {
            case "price_desc":
                products = products.OrderByDescending(p => p.Price);
                break;
            case "PriceAsc":
                products = products.OrderBy(p => p.Price);
                break;
            case "price_asc":
                products = products.OrderBy(p => p.Price);
                break;
            default:
                products = products.OrderBy(p => p.Name);
                break;
        }

        return View(products.ToList());
    }

    [HttpPost]
    public IActionResult AddToCart(int productId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out var userId))
        {
            var product = _context.Products.Find(productId); 
            if (product != null)
            {
                var basketItem = _context.Baskets
                    .FirstOrDefault(b => b.IdProduct == productId && b.IdUser == userId);

                if (basketItem != null)
                {
                    basketItem.Quantity++;
                }
                else
                {
                    basketItem = new Basket
                    {
                        IdProduct = productId,
                        Quantity = 1,
                        IdUser = userId
                    };
                    _context.Baskets.Add(basketItem);
                }

                _context.SaveChanges();
            }
        }

        return RedirectToAction("Index");
    }

    public IActionResult Cart()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out var userId))
        {
            var basketItems = _context.Baskets
                .Include(b => b.Product) 
                .ThenInclude(p => p.Brand) 
                .Where(b => b.IdUser == userId)
                .ToList();

            return View(basketItems);
        }
        return RedirectToAction("Index");
    }

    
    [HttpPost]
    public IActionResult UpdateCart(int productId, int quantity)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out var userId))
        {
            var basketItem = _context.Baskets
                .FirstOrDefault(b => b.IdProduct == productId && b.IdUser == userId);

            if (basketItem != null)
            {
                if (quantity > 0)
                {
                    basketItem.Quantity = quantity;
                }
                else
                {
                    _context.Baskets.Remove(basketItem);
                }

                _context.SaveChanges();
            }
        }

        return RedirectToAction("Cart");
    }

    
    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out var userId))
        {
            var basketItem = _context.Baskets
                .FirstOrDefault(b => b.IdProduct == productId && b.IdUser == userId);

            if (basketItem != null)
            {
                _context.Baskets.Remove(basketItem);
                _context.SaveChanges();
            }
        }

        return RedirectToAction("Cart");
    }
}
