namespace KhoDuoc.DTOs
{
    public class PhieuNhapCreateDto
    {
        public string? SoPhieu { get; set; }
        public DateTime? NgayNhap { get; set; }
        public int? MaKho { get; set; }
        public int? MaNCC { get; set; }
        public int? MaUser { get; set; }
        public List<ChiTietPhieuNhapDto> ChiTiet { get; set; } = new();
    }

    public class ChiTietPhieuNhapDto
    {
        public int MaLo { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }

    public class PhieuNhapResponseDto
    {
        public int MaPhieuNhap { get; set; }
        public string? SoPhieu { get; set; }
        public DateTime NgayNhap { get; set; }
        public string? TenKho { get; set; }
        public string? TenNCC { get; set; }
        public string? NguoiNhap { get; set; }
        public decimal? TongTien { get; set; }
        public List<ChiTietPhieuNhapResponseDto> ChiTiet { get; set; } = new();
    }

    public class ChiTietPhieuNhapResponseDto
    {
        public int MaCTPN { get; set; }
        public string? TenThuoc { get; set; }
        public string? SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}