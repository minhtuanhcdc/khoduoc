
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("ChiTietPhieuXuat")]
    public class ChiTietPhieuXuat
    {
        [Key]
        public int MaCTPX { get; set; }

        public int? MaPhieuXuat { get; set; }

        public int? MaLo { get; set; }

        [Range(1, int.MaxValue)]
        public int? SoLuong { get; set; }

        public decimal? DonGia { get; set; }

        // Navigation properties
        [ForeignKey("MaPhieuXuat")]
        public virtual PhieuXuat? PhieuXuat { get; set; }

        [ForeignKey("MaLo")]
        public virtual LoThuoc? LoThuoc { get; set; }
    }
}