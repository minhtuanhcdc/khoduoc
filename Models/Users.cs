
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int MaUser { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? HoTen { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public int? MaDonVi { get; set; }

        public int? MaCoSo { get; set; }

        public int? MaVaiTro { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("MaDonVi")]
        public virtual DonVi? DonVi { get; set; }

        [ForeignKey("MaCoSo")]
        public virtual CoSo? CoSo { get; set; }

        [ForeignKey("MaVaiTro")]
        public virtual VaiTro? VaiTro { get; set; }

        public virtual ICollection<PhieuNhap>? PhieuNhaps { get; set; }
        public virtual ICollection<PhieuXuat>? PhieuXuats { get; set; }
        public virtual ICollection<YeuCauThuoc>? YeuCauThuocs { get; set; }
    }
}