namespace KhoDuoc.DTOs
{
    public class TonKhoChiTietDto
    {
        public string? TenDonVi { get; set; }
        public string? TenCoSo { get; set; }
        public string? TenKho { get; set; }
        public string? TenThuoc { get; set; }
        public string? SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public int SoLuongTon { get; set; }
    }

    public class ThuocSapHetHanDto
    {
        public int MaLo { get; set; }
        public string? TenThuoc { get; set; }
        public string? SoLo { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? HanSuDung { get; set; }
        public int SoLuongTon { get; set; }
        public int SoNgayConLai { get; set; }
    }
}
