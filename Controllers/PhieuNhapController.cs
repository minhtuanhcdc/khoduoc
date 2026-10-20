using KhoDuoc.Data;
using KhoDuoc.DTOs;
using KhoDuoc.Models;
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
    public class PhieuNhapController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhieuNhapController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int? maKho)
        {
            var query = _context.PhieuNhaps
                .Include(p => p.Kho)
                .Include(p => p.NhaCungCap)
                .Include(p => p.User)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(p => p.NgayNhap >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(p => p.NgayNhap <= toDate.Value);
            if (maKho.HasValue)
                query = query.Where(p => p.MaKho == maKho);

            var result = await query
                .OrderByDescending(p => p.NgayNhap)
                .Select(p => new
                {
                    p.MaPhieuNhap,
                    p.SoPhieu,
                    p.NgayNhap,
                    TenKho = p.Kho != null ? p.Kho.TenKho : null,
                    TenNCC = p.NhaCungCap != null ? p.NhaCungCap.TenNCC : null,
                    NguoiNhap = p.User != null ? p.User.HoTen : null,
                    p.TongTien
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.Kho)
                .Include(p => p.NhaCungCap)
                .Include(p => p.User)
                .Include(p => p.ChiTietPhieuNhaps!)
                    .ThenInclude(ct => ct.LoThuoc)
                    .ThenInclude(l => l!.Thuoc)
                .FirstOrDefaultAsync(p => p.MaPhieuNhap == id);

            if (phieuNhap == null)
                return NotFound();

            var result = new PhieuNhapResponseDto
            {
                MaPhieuNhap = phieuNhap.MaPhieuNhap,
                SoPhieu = phieuNhap.SoPhieu,
                NgayNhap = phieuNhap.NgayNhap,
                TenKho = phieuNhap.Kho?.TenKho,
                TenNCC = phieuNhap.NhaCungCap?.TenNCC,
                NguoiNhap = phieuNhap.User?.HoTen,
                TongTien = phieuNhap.TongTien,
                ChiTiet = phieuNhap.ChiTietPhieuNhaps?.Select(ct => new ChiTietPhieuNhapResponseDto
                {
                    MaCTPN = ct.MaCTPN,
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
        public async Task<IActionResult> Create([FromBody] PhieuNhapCreateDto dto)
        {
            // Validate request
            if (dto.MaKho == null || dto.MaNCC == null || dto.MaUser == null)
            {
                return BadRequest(new
                {
                    message = "MaKho, MaNCC, MaUser không được để trống"
                });
            }

            if (dto.ChiTiet == null || !dto.ChiTiet.Any())
            {
                return BadRequest(new
                {
                    message = "Phiếu nhập phải có ít nhất 1 chi tiết"
                });
            }

            // Check tồn tại FK master
            var khoExists = await _context.Khos.AnyAsync(x => x.MaKho == dto.MaKho);
            if (!khoExists)
                return BadRequest(new { message = $"Kho {dto.MaKho} không tồn tại" });

            var nccExists = await _context.NhaCungCaps.AnyAsync(x => x.MaNCC == dto.MaNCC);
            if (!nccExists)
                return BadRequest(new { message = $"Nhà cung cấp {dto.MaNCC} không tồn tại" });

            var userExists = await _context.Users.AnyAsync(x => x.MaUser == dto.MaUser);
            if (!userExists)
                return BadRequest(new { message = $"User {dto.MaUser} không tồn tại" });

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var phieuNhap = new PhieuNhap
                {
                    SoPhieu = dto.SoPhieu ?? $"PN{DateTime.Now:yyyyMMddHHmmss}",
                    NgayNhap = dto.NgayNhap ?? DateTime.Now,
                    MaKho = dto.MaKho,
                    MaNCC = dto.MaNCC,
                    MaUser = dto.MaUser,
                    TongTien = 0
                };

                _context.PhieuNhaps.Add(phieuNhap);
                await _context.SaveChangesAsync();

                decimal tongTien = 0;

                foreach (var item in dto.ChiTiet)
                {
                    var loExists = await _context.LoThuocs
                        .AnyAsync(x => x.MaLo == item.MaLo);

                    if (!loExists)
                        throw new Exception($"Mã lô {item.MaLo} không tồn tại");

                    var chiTiet = new ChiTietPhieuNhap
                    {
                        MaPhieuNhap = phieuNhap.MaPhieuNhap,
                        MaLo = item.MaLo,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia
                    };

                    _context.ChiTietPhieuNhaps.Add(chiTiet);
                    tongTien += item.SoLuong * item.DonGia;
                }

                phieuNhap.TongTien = tongTien;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    MaPhieuNhap = phieuNhap.MaPhieuNhap,
                    SoPhieu = phieuNhap.SoPhieu,
                    NgayNhap = phieuNhap.NgayNhap,
                    MaKho = phieuNhap.MaKho,
                    MaNCC = phieuNhap.MaNCC,
                    MaUser = phieuNhap.MaUser,
                    TongTien = phieuNhap.TongTien
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new
                {
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var details = await _context.ChiTietPhieuNhaps
                .Where(ct => ct.MaPhieuNhap == id)
                .Select(ct => new
                {
                    ct.MaCTPN,
                    TenThuoc = ct.LoThuoc!.Thuoc!.TenThuoc,
                    SoLo = ct.LoThuoc.SoLo,
                    HanSuDung = ct.LoThuoc.HanSuDung,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = (ct.SoLuong ?? 0) * (ct.DonGia ?? 0)
                })
                .ToListAsync();

            return Ok(details);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PhieuNhapCreateDto dto)
        {
            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.ChiTietPhieuNhaps)
                .FirstOrDefaultAsync(p => p.MaPhieuNhap == id);

            if (phieuNhap == null)
                return NotFound(new { message = "Phiếu nhập không tồn tại" });

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // cập nhật master
                phieuNhap.SoPhieu = dto.SoPhieu ?? phieuNhap.SoPhieu;
                phieuNhap.NgayNhap = dto.NgayNhap ?? phieuNhap.NgayNhap;
                phieuNhap.MaKho = dto.MaKho;
                phieuNhap.MaNCC = dto.MaNCC;
                phieuNhap.MaUser = dto.MaUser;

                // xóa chi tiết cũ
                if (phieuNhap.ChiTietPhieuNhaps != null &&
                    phieuNhap.ChiTietPhieuNhaps.Any())
                {
                    _context.ChiTietPhieuNhaps.RemoveRange(phieuNhap.ChiTietPhieuNhaps);
                }

                decimal tongTien = 0;

                // thêm chi tiết mới
                foreach (var item in dto.ChiTiet)
                {
                    var loExists = await _context.LoThuocs
                        .AnyAsync(x => x.MaLo == item.MaLo);

                    if (!loExists)
                        throw new Exception($"Mã lô {item.MaLo} không tồn tại");

                    var chiTiet = new ChiTietPhieuNhap
                    {
                        MaPhieuNhap = id,
                        MaLo = item.MaLo,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia
                    };

                    _context.ChiTietPhieuNhaps.Add(chiTiet);
                    tongTien += item.SoLuong * item.DonGia;
                }

                phieuNhap.TongTien = tongTien;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Cập nhật phiếu nhập thành công",
                    phieuNhap.MaPhieuNhap
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new
                {
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.ChiTietPhieuNhaps)
                .FirstOrDefaultAsync(p => p.MaPhieuNhap == id);

            if (phieuNhap == null)
                return NotFound(new { message = "Phiếu nhập không tồn tại" });

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // xóa chi tiết
                if (phieuNhap.ChiTietPhieuNhaps != null &&
                    phieuNhap.ChiTietPhieuNhaps.Any())
                {
                    _context.ChiTietPhieuNhaps.RemoveRange(phieuNhap.ChiTietPhieuNhaps);
                }

                // xóa master
                _context.PhieuNhaps.Remove(phieuNhap);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Xóa phiếu nhập thành công"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new
                {
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
    }
}