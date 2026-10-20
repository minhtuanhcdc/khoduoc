
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KhoDuoc.Models
{
    [Table("PhieuNhap")]
    public class PhieuNhap
    {
        [Key]
        public int MaPhieuNhap { get; set; }

        [MaxLength(50)]
        public string? SoPhieu { get; set; }

        public DateTime NgayNhap { get; set; } = DateTime.Now;

        public int? MaKho { get; set; }

        public int? MaNCC { get; set; }

        public int? MaUser { get; set; }

        public decimal? TongTien { get; set; }

        // Navigation properties
        [ForeignKey("MaKho")]
        public virtual Kho? Kho { get; set; }

        [ForeignKey("MaNCC")]
        public virtual NhaCungCap? NhaCungCap { get; set; }

        [ForeignKey("MaUser")]
        public virtual User? User { get; set; }

        [JsonIgnore]
        public virtual ICollection<ChiTietPhieuNhap>? ChiTietPhieuNhaps { get; set; }
    }
}