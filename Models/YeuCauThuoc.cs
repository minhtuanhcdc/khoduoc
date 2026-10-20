

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("YeuCauThuoc")]
    public class YeuCauThuoc
    {
        [Key]
        public int MaYeuCau { get; set; }

        [MaxLength(50)]
        public string? SoYeuCau { get; set; }

        public int? TuCoSo { get; set; }

        public int? DenCoSo { get; set; }

        public DateTime NgayYeuCau { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? TrangThai { get; set; }

        public int? MaUser { get; set; }

        // Navigation properties
        [ForeignKey("TuCoSo")]
        public virtual CoSo? CoSoTu { get; set; }

        [ForeignKey("DenCoSo")]
        public virtual CoSo? CoSoDen { get; set; }

        [ForeignKey("MaUser")]
        public virtual User? User { get; set; }

        public virtual ICollection<ChiTietYeuCauThuoc>? ChiTietYeuCauThuocs { get; set; }
    }
}