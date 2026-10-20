using KhoDuoc.Data;
using KhoDuoc.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhoDuoc.Data;
using KhoDuoc.DTOs;

namespace KhoDuoc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TonKhoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TonKhoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? maKho,
            [FromQuery] int? maThuoc)
        {
            var query = _context.TonKhos
                .Include(t => t.Kho)
                .Include(t => t.LoThuoc!)
                    .ThenInclude(l => l.Thuoc)
                .AsQueryable();

            if (maKho.HasValue)
                query = query.Where(t => t.MaKho == maKho);
            if (maThuoc.HasValue)
                query = query.Where(t => t.LoThuoc!.MaThuoc == maThuoc);

            var result = await query
                .Where(t => t.SoLuongTon > 0)
                .Select(t => new
                {
                    t.MaTonKho,
                    TenKho = t.Kho != null ? t.Kho.TenKho : null,
                    TenThuoc = t.LoThuoc != null ? t.LoThuoc.Thuoc!.TenThuoc : null,
                    t.LoThuoc!.SoLo,
                    t.LoThuoc.HanSuDung,
                    t.SoLuongTon,
                    GiaBan = t.LoThuoc.GiaBan
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 10)
        {
            var result = await _context.TonKhos
                .Include(t => t.Kho)
                .Include(t => t.LoThuoc!)
                    .ThenInclude(l => l.Thuoc)
                .Where(t => t.SoLuongTon <= threshold && t.SoLuongTon > 0)
                .Select(t => new TonKhoChiTietDto
                {
                    TenKho = t.Kho != null ? t.Kho.TenKho : null,
                    TenThuoc = t.LoThuoc != null ? t.LoThuoc.Thuoc!.TenThuoc : null,
                    SoLo = t.LoThuoc != null ? t.LoThuoc.SoLo : null,
                    HanSuDung = t.LoThuoc != null ? t.LoThuoc.HanSuDung : null,
                    SoLuongTon = t.SoLuongTon
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("expiring")]
        public async Task<IActionResult> GetExpiring([FromQuery] int days = 30)
        {
            var expiryDate = DateTime.Now.AddDays(days);

            var result = await _context.TonKhos
                .Include(t => t.Kho)
                .Include(t => t.LoThuoc!)
                    .ThenInclude(l => l.Thuoc)
                .Where(t => t.LoThuoc!.HanSuDung <= expiryDate && t.SoLuongTon > 0)
                .Select(t => new ThuocSapHetHanDto
                {
                    MaLo = t.MaLo ?? 0,
                    TenThuoc = t.LoThuoc != null ? t.LoThuoc.Thuoc!.TenThuoc : null,
                    SoLo = t.LoThuoc != null ? t.LoThuoc.SoLo : null,
                    NgaySanXuat = t.LoThuoc != null ? t.LoThuoc.NgaySanXuat : null,
                    HanSuDung = t.LoThuoc != null ? t.LoThuoc.HanSuDung : null,
                    SoLuongTon = t.SoLuongTon,
                    SoNgayConLai = t.LoThuoc != null && t.LoThuoc.HanSuDung.HasValue
                        ? (t.LoThuoc.HanSuDung.Value - DateTime.Now).Days
                        : 0
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("kho/{maKho}")]
        public async Task<IActionResult> GetByKho(int maKho)
        {
            var result = await _context.TonKhos
                .Include(t => t.LoThuoc!)
                    .ThenInclude(l => l.Thuoc)
                .Where(t => t.MaKho == maKho && t.SoLuongTon > 0)
                .Select(t => new
                {
                    TenThuoc = t.LoThuoc != null ? t.LoThuoc.Thuoc!.TenThuoc : null,
                    t.LoThuoc!.SoLo,
                    t.LoThuoc.HanSuDung,
                    t.SoLuongTon,
                    t.LoThuoc.GiaBan
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}