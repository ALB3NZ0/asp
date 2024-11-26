using System;
using System.Collections.Generic;

namespace Shoes_store.Models;

public partial class Favorite
{
    public int IdFavorites { get; set; }

    public int IdProduct { get; set; }

    public int IdUser { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
