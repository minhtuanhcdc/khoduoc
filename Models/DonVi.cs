using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("DonVi")]
    public class DonVi
    {
        [Key]
        public int MaDonVi { get; set; }

        [MaxLength(50)]
        public string?MaDonViCode { get; set; }  // ← Đổi từ TenDonViCode thành MaDonViCode

        [Required]
        [MaxLength(255)]
        public string TenDonVi { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(20)]
        public string? DienThoai { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<CoSo>? CoSos { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}