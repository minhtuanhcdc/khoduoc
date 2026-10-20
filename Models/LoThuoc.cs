
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace KhoDuoc.Models
{
    [Table("LoThuoc")]
    public class LoThuoc
    {
        [Key]
        public int MaLo { get; set; }

        [Required]
        public int MaThuoc { get; set; }

        [MaxLength(100)]
        public string? SoLo { get; set; }

        public DateTime? NgaySanXuat { get; set; }

        public DateTime? HanSuDung { get; set; }

        public decimal GiaNhap { get; set; }

        public decimal GiaBan { get; set; }

        // Navigation properties
        [ForeignKey("MaThuoc")]
        public virtual Thuoc? Thuoc { get; set; }

        [JsonIgnore]
        public virtual ICollection<ChiTietPhieuNhap>? ChiTietPhieuNhaps { get; set; }
        [JsonIgnore]
        public virtual ICollection<ChiTietPhieuXuat>? ChiTietPhieuXuats { get; set; }
        [JsonIgnore]
        public virtual ICollection<TonKho>? TonKhos { get; set; }
    }
}