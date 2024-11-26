using System;
using System.Collections.Generic;

namespace Shoes_store.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? Size { get; set; }

    public bool IsRetro { get; set; }

    public int IdBrand { get; set; }

    public string? ImageUrl { get; set; }


    // Навигационное свойство для связи с брендом
    public Brand Brand { get; set; }  // Корректное навигационное свойство

    // Другие связи с корзинами и отзывами
    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
