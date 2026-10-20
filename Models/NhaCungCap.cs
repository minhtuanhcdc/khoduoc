
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("NhaCungCap")]
    public class NhaCungCap
    {
        [Key]
        public int MaNCC { get; set; }

        [MaxLength(255)]
        public string? TenNCC { get; set; }

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(20)]
        public string? DienThoai { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        // Navigation properties
        public virtual ICollection<Thuoc>? Thuocs { get; set; }
        public virtual ICollection<PhieuNhap>? PhieuNhaps { get; set; }
    }
}