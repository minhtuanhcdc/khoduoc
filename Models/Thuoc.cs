
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("Thuoc")]
    public class Thuoc
    {
        [Key]
        public int MaThuoc { get; set; }

        [MaxLength(50)]
        public string? MaThuocCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string TenThuoc { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? DonViTinh { get; set; }

        [MaxLength(100)]
        public string? HamLuong { get; set; }

        [MaxLength(100)]
        public string? DangBaoChe { get; set; }

        public int? MaNCC { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("MaNCC")]
        public virtual NhaCungCap? NhaCungCap { get; set; }

        public virtual ICollection<LoThuoc>? LoThuocs { get; set; }
        public virtual ICollection<ChiTietYeuCauThuoc>? ChiTietYeuCauThuocs { get; set; }
    }
}