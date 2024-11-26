using Microsoft.AspNetCore.Mvc;
using Shoes_store.Models;
using System.Diagnostics;

namespace Shoes_store.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShoppingDbContext _context;

        public HomeController(ILogger<HomeController> logger, ShoppingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Проверяем авторизацию пользователя
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Пожалуйста, войдите в систему."; // Сообщение об ошибке
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Пожалуйста, войдите в систему."; // Сообщение об ошибке
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
