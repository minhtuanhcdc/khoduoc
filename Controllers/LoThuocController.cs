using KhoDuoc.Data;
using KhoDuoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhoDuoc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoThuocController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoThuocController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LoThuoc
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Select(l => new
                {
                    l.MaLo,
                    l.MaThuoc,
                    TenThuoc = l.Thuoc != null ? l.Thuoc.TenThuoc : null,
                    l.SoLo,
                    l.NgaySanXuat,
                    l.HanSuDung,
                    l.GiaNhap,
                    l.GiaBan
                })
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/LoThuoc/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var loThuoc = await _context.LoThuocs
                .Include(l => l.Thuoc)
                .Where(l => l.MaLo == id)
                .Select(l => new
                {
                    l.MaLo,
                    l.MaThuoc,
                    TenThuoc = l.Thuoc != null ? l.Thuoc.TenThuoc : null,
                    l.SoLo,
                    l.NgaySanXuat,
                    l.HanSuDung,
                    l.GiaNhap,
                    l.GiaBan
                })
                .FirstOrDefaultAsync();

            if (loThuoc == null)
                return NotFound(new { message = "Không tìm thấy lô thuốc" });

            return Ok(loThuoc);
        }

        // GET: api/LoThuoc/by-thuoc/1
        [HttpGet("by-thuoc/{maThuoc}")]
        public async Task<IActionResult> GetByThuoc(int maThuoc)
        {
            var result = await _context.LoThuocs
                .Where(l => l.MaThuoc == maThuoc)
                .Select(l => new
                {
                    l.MaLo,
                    l.SoLo,
                    l.HanSuDung,
                    l.GiaNhap,
                    l.GiaBan
                })
                .ToListAsync();

            return Ok(result);
        }

        // POST: api/LoThuoc
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoThuoc loThuoc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var thuocExists = await _context.Thuocs.AnyAsync(t => t.MaThuoc == loThuoc.MaThuoc);
            if (!thuocExists)
                return BadRequest(new { message = "Mã thuốc không tồn tại" });

            if (loThuoc.NgaySanXuat.HasValue && loThuoc.HanSuDung.HasValue
                && loThuoc.HanSuDung <= loThuoc.NgaySanXuat)
            {
                return BadRequest(new
                {
                    message = "Hạn sử dụng phải lớn hơn ngày sản xuất"
                });
            }

            if (loThuoc.GiaNhap < 0 || loThuoc.GiaBan < 0)
            {
                return BadRequest(new
                {
                    message = "Giá nhập và giá bán phải >= 0"
                });
            }

            _context.LoThuocs.Add(loThuoc);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = loThuoc.MaLo }, loThuoc);
        }

        // PUT: api/LoThuoc/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LoThuoc loThuoc)
        {
            if (id != loThuoc.MaLo)
                return BadRequest(new { message = "ID không khớp" });

            var existing = await _context.LoThuocs.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Không tìm thấy lô thuốc" });

            var thuocExists = await _context.Thuocs.AnyAsync(t => t.MaThuoc == loThuoc.MaThuoc);
            if (!thuocExists)
                return BadRequest(new { message = "Mã thuốc không tồn tại" });

            if (loThuoc.NgaySanXuat.HasValue && loThuoc.HanSuDung.HasValue
                && loThuoc.HanSuDung <= loThuoc.NgaySanXuat)
            {
                return BadRequest(new
                {
                    message = "Hạn sử dụng phải lớn hơn ngày sản xuất"
                });
            }

            existing.MaThuoc = loThuoc.MaThuoc;
            existing.SoLo = loThuoc.SoLo;
            existing.NgaySanXuat = loThuoc.NgaySanXuat;
            existing.HanSuDung = loThuoc.HanSuDung;
            existing.GiaNhap = loThuoc.GiaNhap;
            existing.GiaBan = loThuoc.GiaBan;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // DELETE: api/LoThuoc/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var loThuoc = await _context.LoThuocs.FindAsync(id);

            if (loThuoc == null)
                return NotFound(new { message = "Không tìm thấy lô thuốc" });

            var hasRelated = await _context.ChiTietPhieuNhaps
                .AnyAsync(c => c.MaLo == id);

            if (hasRelated)
            {
                return BadRequest(new
                {
                    message = "Không thể xóa vì lô thuốc đã phát sinh phiếu nhập"
                });
            }

            _context.LoThuocs.Remove(loThuoc);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
