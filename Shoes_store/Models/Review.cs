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

    public virtual Product IdProductNavigation { get; set; } = null!;
}
