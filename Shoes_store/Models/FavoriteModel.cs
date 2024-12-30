namespace Shoes_store.Models
{
    public class FavoriteModel
    {
        public int IdFavorites { get; set; }
        public int IdProduct { get; set; }
        public int IdUser { get; set; }

        // Навигационные свойства
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
