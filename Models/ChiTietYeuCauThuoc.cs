
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("ChiTietYeuCauThuoc")]
    public class ChiTietYeuCauThuoc
    {
        [Key]
        public int MaCTYC { get; set; }

        public int? MaYeuCau { get; set; }

        public int? MaThuoc { get; set; }

        [Range(1, int.MaxValue)]
        public int? SoLuong { get; set; }

        // Navigation properties
        [ForeignKey("MaYeuCau")]
        public virtual YeuCauThuoc? YeuCauThuoc { get; set; }

        [ForeignKey("MaThuoc")]
        public virtual Thuoc? Thuoc { get; set; }
    }
}