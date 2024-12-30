using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shoes_store.Models;

public partial class Brand
{
    public int IdBrand { get; set; }

    [Required]
    public string BrandName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
