using System;
using System.Collections.Generic;

namespace Shoes_store.Models;

public partial class Review
{
    public int IdReview { get; set; }

    public int IdProduct { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime ReviewDate { get; set; }

    public int? IdUser { get; set; }

    // Навигационные свойства

    public virtual Product Product { get; set; } = null!;

    public virtual User? User { get; set; }
}
