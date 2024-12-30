using System.ComponentModel.DataAnnotations;

namespace Shoes_store.Models
{
    public class BrandModel
    {
        public int IdBrand { get; set; }

        [Required]
        public string BrandName { get; set; } = null!;

        public string? Description { get; set; }
    }
}
