
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("PhieuXuat")]
    public class PhieuXuat
    {
        [Key]
        public int MaPhieuXuat { get; set; }

        [MaxLength(50)]
        public string? SoPhieu { get; set; }

        public DateTime NgayXuat { get; set; } = DateTime.Now;

        public int? MaKho { get; set; }

        [MaxLength(255)]
        public string? NoiNhan { get; set; }

        public int? TuCoSo { get; set; }

        public int? DenCoSo { get; set; }

        public int? MaUser { get; set; }

        public decimal? TongTien { get; set; }

        // Navigation properties
        [ForeignKey("MaKho")]
        public virtual Kho? Kho { get; set; }

        [ForeignKey("TuCoSo")]
        public virtual CoSo? CoSoTu { get; set; }

        [ForeignKey("DenCoSo")]
        public virtual CoSo? CoSoDen { get; set; }

        [ForeignKey("MaUser")]
        public virtual User? User { get; set; }

        public virtual ICollection<ChiTietPhieuXuat>? ChiTietPhieuXuats { get; set; }
    }
}