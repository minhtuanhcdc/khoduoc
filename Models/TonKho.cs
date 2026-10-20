
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("TonKho")]
    public class TonKho
    {
        [Key]
        public int MaTonKho { get; set; }

        public int? MaKho { get; set; }

        public int? MaLo { get; set; }

        public int SoLuongTon { get; set; } = 0;

        // Navigation properties
        [ForeignKey("MaKho")]
        public virtual Kho? Kho { get; set; }

        [ForeignKey("MaLo")]
        public virtual LoThuoc? LoThuoc { get; set; }
    }
}