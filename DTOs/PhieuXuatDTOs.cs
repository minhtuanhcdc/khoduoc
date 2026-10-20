namespace KhoDuoc.DTOs
{
    public class PhieuXuatCreateDto
    {
        public string? SoPhieu { get; set; }
        public DateTime? NgayXuat { get; set; }
        public int? MaKho { get; set; }
        public string? NoiNhan { get; set; }
        public int? TuCoSo { get; set; }
        public int? DenCoSo { get; set; }
        public int? MaUser { get; set; }
        public List<ChiTietPhieuXuatDto> ChiTiet { get; set; } = new();
    }

    public class ChiTietPhieuXuatDto
    {
        public int MaLo { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }

    public class PhieuXuatResponseDto
    {
        public int MaPhieuXuat { get; set; }
        public string? SoPhieu { get; set; }
        public DateTime NgayXuat { get; set; }
        public string? TenKho { get; set; }
        public string? NoiNhan { get; set; }
        public string? TuCoSo { get; set; }
        public string? DenCoSo { get; set; }
        public string? NguoiXuat { get; set; }
        public decimal? TongTien { get; set; }
        public List<ChiTietPhieuXuatResponseDto> ChiTiet { get; set; } = new();
    }

    public class ChiTietPhieuXuatResponseDto
    {
        public int MaCTPX { get; set; }
        public string? TenThuoc { get; set; }
        public string? SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}