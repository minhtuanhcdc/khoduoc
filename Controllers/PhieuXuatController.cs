using KhoDuoc.Data;
using KhoDuoc.DTOs;
using KhoDuoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhoDuoc.Data;
using KhoDuoc.DTOs;
using KhoDuoc.Models;

namespace KhoDuoc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PhieuXuatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhieuXuatController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int? maKho)
        {
            var query = _context.PhieuXuats
                .Include(p => p.Kho)
                .Include(p => p.User)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(p => p.NgayXuat >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(p => p.NgayXuat <= toDate.Value);
            if (maKho.HasValue)
                query = query.Where(p => p.MaKho == maKho);

            var result = await query
                .OrderByDescending(p => p.NgayXuat)
                .Select(p => new
                {
                    p.MaPhieuXuat,
                    p.SoPhieu,
                    p.NgayXuat,
                    TenKho = p.Kho != null ? p.Kho.TenKho : null,
                    p.NoiNhan,
                    NguoiXuat = p.User != null ? p.User.HoTen : null,
                    p.TongTien
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var phieuXuat = await _context.PhieuXuats
                .Include(p => p.Kho)
                .Include(p => p.CoSoTu)
                .Include(p => p.CoSoDen)
                .Include(p => p.User)
                .Include(p => p.ChiTietPhieuXuats!)
                    .ThenInclude(ct => ct.LoThuoc)
                    .ThenInclude(l => l!.Thuoc)
                .FirstOrDefaultAsync(p => p.MaPhieuXuat == id);

            if (phieuXuat == null)
                return NotFound();

            var result = new PhieuXuatResponseDto
            {
                MaPhieuXuat = phieuXuat.MaPhieuXuat,
                SoPhieu = phieuXuat.SoPhieu,
                NgayXuat = phieuXuat.NgayXuat,
                TenKho = phieuXuat.Kho?.TenKho,
                NoiNhan = phieuXuat.NoiNhan,
                TuCoSo = phieuXuat.CoSoTu?.TenCoSo,
                DenCoSo = phieuXuat.CoSoDen?.TenCoSo,
                NguoiXuat = phieuXuat.User?.HoTen,
                TongTien = phieuXuat.TongTien,
                ChiTiet = phieuXuat.ChiTietPhieuXuats?.Select(ct => new ChiTietPhieuXuatResponseDto
                {
                    MaCTPX = ct.MaCTPX,
                    TenThuoc = ct.LoThuoc?.Thuoc?.TenThuoc,
                    SoLo = ct.LoThuoc?.SoLo,
                    HanSuDung = ct.LoThuoc?.HanSuDung,
                    SoLuong = ct.SoLuong ?? 0,
                    DonGia = ct.DonGia ?? 0,
                    ThanhTien = (ct.SoLuong ?? 0) * (ct.DonGia ?? 0)
                }).ToList() ?? new()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhieuXuatCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check inventory before creating
                foreach (var item in dto.ChiTiet)
                {
                    var tonKho = await _context.TonKhos
                        .FirstOrDefaultAsync(t => t.MaKho == dto.MaKho && t.MaLo == item.MaLo);

                    if (tonKho == null || tonKho.SoLuongTon < item.SoLuong)
                    {
                        return BadRequest(new { message = $"Insufficient stock for lot {item.MaLo}" });
                    }
                }

                var phieuXuat = new PhieuXuat
                {
                    SoPhieu = dto.SoPhieu ?? $"PX{DateTime.Now:yyyyMMddHHmmss}",
                    NgayXuat = dto.NgayXuat ?? DateTime.Now,
                    MaKho = dto.MaKho,
                    NoiNhan = dto.NoiNhan,
                    TuCoSo = dto.TuCoSo,
                    DenCoSo = dto.DenCoSo,
                    MaUser = dto.MaUser,
                    TongTien = 0
                };

                _context.PhieuXuats.Add(phieuXuat);
                await _context.SaveChangesAsync();

                decimal tongTien = 0;
                foreach (var item in dto.ChiTiet)
                {
                    var chiTiet = new ChiTietPhieuXuat
                    {
                        MaPhieuXuat = phieuXuat.MaPhieuXuat,
                        MaLo = item.MaLo,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia
                    };
                    _context.ChiTietPhieuXuats.Add(chiTiet);
                    tongTien += item.SoLuong * item.DonGia;
                }

                phieuXuat.TongTien = tongTien;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = phieuXuat.MaPhieuXuat }, phieuXuat);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var details = await _context.ChiTietPhieuXuats
                .Include(ct => ct.LoThuoc!)
                    .ThenInclude(l => l.Thuoc)
                .Where(ct => ct.MaPhieuXuat == id)
                .Select(ct => new
                {
                    ct.MaCTPX,
                    TenThuoc = ct.LoThuoc!.Thuoc!.TenThuoc,
                    ct.LoThuoc.SoLo,
                    ct.LoThuoc.HanSuDung,
                    ct.SoLuong,
                    ct.DonGia,
                    ThanhTien = ct.SoLuong * ct.DonGia
                })
                .ToListAsync();

            return Ok(details);
        }
    }
}