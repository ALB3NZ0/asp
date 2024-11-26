using System;
using System.Collections.Generic;

namespace Shoes_store.Models;

public partial class Brand
{
    public int IdBrand { get; set; }

    public string BrandName { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; }
}
