using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("VaiTro")]
    public class VaiTro
    {
        [Key]
        public int MaVaiTro { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenVaiTro { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<User>? Users { get; set; }
    }
}