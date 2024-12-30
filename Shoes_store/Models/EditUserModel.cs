using System.ComponentModel.DataAnnotations;

namespace Shoes_store.Models
{
    public class EditUserModel
    {
        public int IdUser { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public int RoleId { get; set; }
    }
}
