using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoes_store.Models;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ShoppingDbContext _context;

    public AdminController(ShoppingDbContext context)
    {
        _context = context;
    }


    public IActionResult Index()
    {
        return View(); 
    }


    // Пользователи
    public IActionResult Users()
    {
        var users = _context.Users.ToList();
        return View(users);
    }

    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }
        return View(user);
    }

    public IActionResult EditUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();
        EditUserModel editUserModel = new EditUserModel();
        editUserModel.IdUser = user.IdUser;
        editUserModel.FullName= user.FullName;
        editUserModel.Email= user.Email;
        editUserModel.RoleId = user.RoleId;
        return View(editUserModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserModel user, string RoleName)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users.FindAsync(user.IdUser);
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.RoleId = DetermineRoleId(RoleName);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }
            ModelState.AddModelError("", "Пользователь не найден.");
        }
        return View(user);
    }

    // Бренды
    public IActionResult Brands()
    {
        var brands = _context.Brands.ToList();
        return View(brands);
    }

    public IActionResult CreateBrand()
    {
        return View();
    }

    public bool IsValidBrandModel(Brand brand)
    {
        if(brand == null)
        {
            return false;
        }

        if(brand.IdBrand != null && !String.IsNullOrEmpty(brand.BrandName) && !String.IsNullOrEmpty(brand.Description) && !String.IsNullOrWhiteSpace(brand.BrandName) && !String.IsNullOrWhiteSpace(brand.Description))
        {
            return true;
        }

        return false;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBrand(Brand brand)
    {
        if (IsValidBrandModel(brand))
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Brands));
        }
        return View(brand);
    }

    public IActionResult EditBrand(int id)
    {
        var brand = _context.Brands.Find(id);
        if (brand == null) return NotFound();
        return View(brand);
    }

    [HttpPost]
    public async Task<IActionResult> EditBrand(Brand brand)
    {
        if (IsValidBrandModel(brand))
        {
            var existingBrand = await _context.Brands.FindAsync(brand.IdBrand);
            if (existingBrand != null)
            {
                existingBrand.BrandName = brand.BrandName;
                existingBrand.Description = brand.Description;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Brands));
            }
            ModelState.AddModelError("", "Бренд не найден.");
        }
        return View(brand);
    }

    // Продукты
    public IActionResult Products()
    {
        var products = _context.Products.Include(p => p.Brand).ToList();
        return View(products);
    }


    public bool IsValidProductModel(Product product)
    {
        if (product == null)
        {
            return false;
        }

        if (!String.IsNullOrEmpty(product.Name) && !String.IsNullOrEmpty(product.Description) && product.Price != null && product.Stock != null &&  !String.IsNullOrWhiteSpace(product.Size) && product.IsRetro != null && product.IdBrand != null)
        {
            return true;
        }

        return false;
    }



    public IActionResult CreateProduct()
    {
        ViewBag.Brands = _context.Brands.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        if (IsValidProductModel(product))
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Products));
        }
        ViewBag.Brands = _context.Brands.ToList();
        return View(product);
    }

    public IActionResult EditProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();
        ViewBag.Brands = _context.Brands.ToList();
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct(Product product)
    {
        if (IsValidProductModel(product))
        {
            var existingProduct = await _context.Products.FindAsync(product.IdProduct);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.Size = product.Size;
                existingProduct.IsRetro = product.IsRetro;
                existingProduct.IdBrand = product.IdBrand;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Products));
            }
            ModelState.AddModelError("", "Продукт не найден.");
        }
        ViewBag.Brands = _context.Brands.ToList();
        return View(product);
    }

    
    private int DetermineRoleId(string roleName)
    {
        return roleName.ToLower() switch
        {
            "admin" => 2,
            "user" => 1,
            _ => 3 // Значение по умолчанию
        };
    }


    // Действия для Basket
    public IActionResult Basket()
    {
        var baskets = _context.Baskets.Include(b => b.Product).Include(b => b.User).ToList();
        return View(baskets);
    }


    public bool IsValidBasketModel(Basket basket)
    {
        if (basket == null)
        {
            return false;
        }

        if (basket.IdBasket != null && basket.IdProduct != null && basket.IdUser != null && basket.Quantity != null)
        {
            return true;
        }

        return false;
    }


    public IActionResult CreateBasket()
    {
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateBasket(Basket basket)
    {
        if (IsValidBasketModel(basket))
        {
            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Basket));
        }
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View(basket);
    }

    public IActionResult EditBasket(int id)
    {
        var basket = _context.Baskets.Find(id);
        if (basket == null) return NotFound();
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View(basket);
    }

    [HttpPost]
    public async Task<IActionResult> EditBasket(Basket basket)
    {
        if (IsValidBasketModel(basket))
        {
            var existingBasket = await _context.Baskets.FindAsync(basket.IdBasket);
            if (existingBasket != null)
            {
                existingBasket.IdProduct = basket.IdProduct;
                existingBasket.Quantity = basket.Quantity;
                existingBasket.IdUser = basket.IdUser;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Basket));
            }
            ModelState.AddModelError("", "Запись корзины не найдена.");
        }
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View(basket);
    }

    public IActionResult DeleteBasket(int id)
    {
        var basket = _context.Baskets.Find(id);
        if (basket == null) return NotFound();
        return View(basket);
    }

    [HttpPost, ActionName("DeleteBasket")]
    public async Task<IActionResult> DeleteBasketConfirmed(int id)
    {
        var basket = await _context.Baskets.FindAsync(id);
        if (basket != null)
        {
            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Basket));
    }

    // Действия для Favorites
    public IActionResult Favorites()
    {
        var favorites = _context.Favorites.Include(f => f.Product).Include(f => f.User).ToList();
        return View(favorites);
    }

    public bool IsValidFavoriteModel(Favorite favorite)
    {
        if (favorite == null)
        {
            return false;
        }

        if (favorite.IdFavorites != null && favorite.IdProduct != null && favorite.IdUser != null)
        {
            return true;
        }

        return false;
    }

    public IActionResult CreateFavorite()
    {
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateFavorite(Favorite favorite)
    {
        if (IsValidFavoriteModel(favorite))
        {
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Favorites));
        }
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View(favorite);
    }

    
    public IActionResult EditFavorite(int id)
    {
        var favorite = _context.Favorites
            .Include(f => f.Product)
            .Include(f => f.User)
            .FirstOrDefault(f => f.IdFavorites == id);

        if (favorite == null)
        {
            return NotFound();
        }

        
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();

        return View(favorite);
    }

    [HttpPost]
    public async Task<IActionResult> EditFavorite(Favorite favorite)
    {
        if (IsValidFavoriteModel(favorite))
        {
            var existingFavorite = await _context.Favorites.FindAsync(favorite.IdFavorites);
            if (existingFavorite != null)
            {
                existingFavorite.IdProduct = favorite.IdProduct;
                existingFavorite.IdUser = favorite.IdUser;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Favorites));
            }

            ModelState.AddModelError("", "Избранный товар не найден.");
        }

        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View(favorite);
    }


    public IActionResult DeleteFavorite(int id)
    {
        var favorite = _context.Favorites.Find(id);
        if (favorite == null) return NotFound();
        return View(favorite);
    }

    [HttpPost, ActionName("DeleteFavorite")]
    public async Task<IActionResult> DeleteFavoriteConfirmed(int id)
    {
        var favorite = await _context.Favorites.FindAsync(id);
        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Favorites));
    }

    // Действия для Reviews
    public IActionResult Reviews()
    {
        var reviews = _context.Reviews.Include(r => r.Product).Include(r => r.User).ToList();
        return View(reviews);
    }


    public bool IsValidReviewsModel(Review review)
    {
        if (review == null)
        {
            return false;
        }

        if (review.IdReview != null && review.IdProduct != null && review.Rating != null && !String.IsNullOrEmpty(review.Comment) && review.IdUser!= null && review.ReviewDate != null)
        {
            return true;
        }

        return false;
    }

    public IActionResult CreateReview()
    {
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview(Review review)
    {
        if (IsValidReviewsModel(review))
        {
            review.ReviewDate = DateTime.Now;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Reviews));
        }
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View(review);
    }

    public IActionResult EditReview(int id)
    {
        var review = _context.Reviews.Find(id);
        if (review == null) return NotFound();
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View(review);
    }

    [HttpPost]
    public async Task<IActionResult> EditReview(Review review)
    {
        if (IsValidReviewsModel(review))
        {
            var existingReview = await _context.Reviews.FindAsync(review.IdReview);
            if (existingReview != null)
            {
                existingReview.Rating = review.Rating;
                existingReview.Comment = review.Comment;
                existingReview.IdProduct = review.IdProduct;
                existingReview.IdUser = review.IdUser;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Reviews));
            }
            ModelState.AddModelError("", "Отзыв не найден.");
        }
        ViewBag.Users = _context.Users.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View(review);
    }

    public IActionResult DeleteReview(int id)
    {
        var review = _context.Reviews.Find(id);
        if (review == null) return NotFound();
        return View(review);
    }

    [HttpPost, ActionName("DeleteReview")]
    public async Task<IActionResult> DeleteReviewConfirmed(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Reviews));
    }
}
