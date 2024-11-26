using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Shoes_store.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Регистрация DbContext с использованием строки подключения
builder.Services.AddDbContext<ShoppingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("con")));

// Настройка службы аутентификации
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Путь к методу логина
        options.LogoutPath = "/Auth/Logout"; // Путь к методу логаута
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Путь к странице доступа
    });

var app = builder.Build();

// Добавьте аутентификацию
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"); // Перенаправляем на страницу Login


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Подключение аутентификации и авторизации
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
