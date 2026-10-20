namespace KhoThuoc.DTOs
{
    public class YeuCauThuocCreateDto
    {
        public string? SoYeuCau { get; set; }
        public int? TuCoSo { get; set; }
        public int? DenCoSo { get; set; }
        public int? MaUser { get; set; }
        public List<ChiTietYeuCauDto> ChiTiet { get; set; } = new();
    }

    public class ChiTietYeuCauDto
    {
        public int MaThuoc { get; set; }
        public int SoLuong { get; set; }
    }

    public class YeuCauThuocResponseDto
    {
        public int MaYeuCau { get; set; }
        public string? SoYeuCau { get; set; }
        public string? TuCoSo { get; set; }
        public string? DenCoSo { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string? TrangThai { get; set; }
        public string? NguoiYeuCau { get; set; }
        public List<ChiTietYeuCauResponseDto> ChiTiet { get; set; } = new();
    }

    public class ChiTietYeuCauResponseDto
    {
        public string? TenThuoc { get; set; }
        public int SoLuong { get; set; }
    }
}