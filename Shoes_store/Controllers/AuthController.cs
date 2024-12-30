using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shoes_store.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class AuthController : Controller
{
    private readonly ShoppingDbContext _context;

    public AuthController(ShoppingDbContext context)
    {
        _context = context;
    }




    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        // Находим пользователя по email и хешу пароля
        var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == HashPassword(password));

        if (user != null)
        {
            // Определяем роль пользователя
            var role = user.RoleId == 2 ? "Admin" : "User"; // 2 — ID для администраторов

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role) // Присваиваем роль
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            // Аутентификация
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Перенаправляем в зависимости от роли
            return RedirectToAction("Index", role == "Admin" ? "Admin" : "Catalog");
        }

        ViewBag.ErrorMessage = "Неверный логин или пароль.";
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string fullName, string email, string password)
    {
        // Проверяем, существует ли пользователь с таким email
        if (_context.Users.Any(u => u.Email == email))
        {
            ViewBag.ErrorMessage = "Пользователь с таким email уже существует.";
            return View();
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = HashPassword(password),
            RoleId = 1 // Задаем роль по умолчанию (обычный пользователь)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login"); // Перенаправляем после регистрации в логин
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
