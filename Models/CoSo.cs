using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhoDuoc.Models
{
    [Table("CoSo")]
    public class CoSo
    {
        [Key]
        public int MaCoSo { get; set; }

        [Required]
        public int MaDonVi { get; set; }

        [MaxLength(50)]
        public string? MaCoSoCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string TenCoSo { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(20)]
        public string? DienThoai { get; set; }

        public bool LaCoSoChinh { get; set; } = false;

        public bool IsActive { get; set; } = true;

        // Navigation properties - CHỈ GIỮ LẠI NHỮNG CÁI NÀY
        [ForeignKey("MaDonVi")]
        public virtual DonVi? DonVi { get; set; }

        public virtual ICollection<Kho>? Khos { get; set; }
        public virtual ICollection<User>? Users { get; set; }

        // COMMENT hoặc XÓA tất cả những dòng dưới đây
        // public virtual ICollection<PhieuXuat>? PhieuXuatTuCoSos { get; set; }
        // public virtual ICollection<PhieuXuat>? PhieuXuatDenCoSos { get; set; }
        // [NotMapped]
        // public virtual ICollection<PhieuXuat>? PhieuXuats { get; set; }
        // public virtual ICollection<YeuCauThuoc>? YeuCauTuCoSos { get; set; }
        // public virtual ICollection<YeuCauThuoc>? YeuCauDenCoSos { get; set; }
    }
}