using System;
using System.Collections.Generic;

namespace Shoes_store.Models;

public partial class Basket
{
    public int IdBasket { get; set; }
    public int IdProduct { get; set; }
    public int IdUser { get; set; }
    public int Quantity { get; set; }

    // Навигационные свойства
    public virtual Product Product { get; set; }
    public virtual User User { get; set; }
}
