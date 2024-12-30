namespace Shoes_store.Models
{
    public class ProductModel
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
        public Brand Brand { get; set; }
    }
}
