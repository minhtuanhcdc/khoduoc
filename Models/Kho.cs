
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("Kho")]
    public class Kho
    {
        [Key]
        public int MaKho { get; set; }

        [Required]
        public int MaCoSo { get; set; }

        [MaxLength(50)]
        public string? MaKhoCode { get; set; }

        [MaxLength(255)]
        public string? TenKho { get; set; }

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        // Navigation properties
        [ForeignKey("MaCoSo")]
        public virtual CoSo? CoSo { get; set; }

        public virtual ICollection<PhieuNhap>? PhieuNhaps { get; set; }
        public virtual ICollection<PhieuXuat>? PhieuXuats { get; set; }
        public virtual ICollection<TonKho>? TonKhos { get; set; }
    }
}