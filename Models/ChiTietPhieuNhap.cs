
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KhoDuoc.Models
{
    [Table("ChiTietPhieuNhap")]
    public class ChiTietPhieuNhap
    {
        [Key]
        public int MaCTPN { get; set; }

        public int? MaPhieuNhap { get; set; }

        public int? MaLo { get; set; }

        [Range(1, int.MaxValue)]
        public int? SoLuong { get; set; }

        public decimal? DonGia { get; set; }

        // Navigation properties
        [ForeignKey("MaPhieuNhap")]
        [JsonIgnore]
        public virtual PhieuNhap? PhieuNhap { get; set; }

        [ForeignKey("MaLo")]
        public virtual LoThuoc? LoThuoc { get; set; }
    }
}