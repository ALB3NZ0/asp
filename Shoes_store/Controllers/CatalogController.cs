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

    // Вывод каталога товаров
    public IActionResult Index()
    {
        var products = _context.Products.ToList(); // Загружаем все продукты
        return View(products);
    }

    // Добавление товара в корзину
    [HttpPost]
    public IActionResult AddToCart(int productId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out var userId))
        {
            var product = _context.Products.Find(productId); // Найти продукт по ID

            if (product != null)
            {
                var basketItem = _context.Baskets
                    .FirstOrDefault(b => b.IdProduct == productId && b.IdUser == userId);

                if (basketItem != null)
                {
                    // Если товар уже в корзине, увеличиваем количество
                    basketItem.Quantity++;
                }
                else
                {
                    // Если товара в корзине нет, создаем новую запись
                    basketItem = new Basket
                    {
                        IdProduct = productId,
                        Quantity = 1,
                        IdUser = userId
                    };
                    _context.Baskets.Add(basketItem);
                }

                _context.SaveChanges(); // Сохраняем изменения
            }
        }

        return RedirectToAction("Index");
    }

    // Отображение корзины
    public IActionResult Cart()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out var userId))
        {
            // Получаем все товары в корзине с информацией о бренде
            var basketItems = _context.Baskets
                .Include(b => b.Product) // Загружаем продукт
                .ThenInclude(p => p.Brand) // Загружаем бренд для каждого продукта
                .Where(b => b.IdUser == userId)
                .ToList();

            return View(basketItems);
        }

        return RedirectToAction("Index");
    }

    // Обновление корзины (изменение количества товара)
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
                    basketItem.Quantity = quantity;  // Обновляем количество товара в корзине
                }
                else
                {
                    _context.Baskets.Remove(basketItem);  // Если количество 0 или меньше, удаляем товар
                }

                _context.SaveChanges();  // Сохраняем изменения в базе данных
            }
        }

        return RedirectToAction("Cart");
    }

    // Удаление товара из корзины
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
                _context.Baskets.Remove(basketItem);  // Удаляем товар из корзины
                _context.SaveChanges();  // Сохраняем изменения
            }
        }

        return RedirectToAction("Cart");
    }
}
