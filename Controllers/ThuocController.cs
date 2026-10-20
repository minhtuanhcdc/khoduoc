using KhoDuoc.Data;
using KhoDuoc.Models;
using KhoDuoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhoDuoc.Data;
using KhoDuoc.Models;

namespace KhoDuoc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ThuocController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThuocController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.Thuocs
                .Include(t => t.NhaCungCap)
                .Where(t => t.IsActive)
                .Select(t => new
                {
                    t.MaThuoc,
                    t.MaThuocCode,
                    t.TenThuoc,
                    t.DonViTinh,
                    t.HamLuong,
                    t.DangBaoChe,
                    NhaCungCap = t.NhaCungCap != null ? t.NhaCungCap.TenNCC : null
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var thuoc = await _context.Thuocs
                .Include(t => t.NhaCungCap)
                .FirstOrDefaultAsync(t => t.MaThuoc == id && t.IsActive);

            if (thuoc == null)
                return NotFound();

            return Ok(thuoc);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Thuoc thuoc)
        {
            thuoc.IsActive = true;
            _context.Thuocs.Add(thuoc);
            await _context.SaveChangesAsync();
            return Ok(thuoc);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Thuoc thuoc)
        {
            if (id != thuoc.MaThuoc)
                return BadRequest();

            var existing = await _context.Thuocs.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.MaThuocCode = thuoc.MaThuocCode;
            existing.TenThuoc = thuoc.TenThuoc;
            existing.DonViTinh = thuoc.DonViTinh;
            existing.HamLuong = thuoc.HamLuong;
            existing.DangBaoChe = thuoc.DangBaoChe;
            existing.MaNCC = thuoc.MaNCC;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc == null)
                return NotFound();

            thuoc.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully" });
        }

        [HttpPost("{id}/lothuoc")]
        public async Task<IActionResult> AddLoThuoc(int id, [FromBody] LoThuoc loThuoc)
        {
            loThuoc.MaThuoc = id;
            _context.LoThuocs.Add(loThuoc);
            await _context.SaveChangesAsync();
            return Ok(loThuoc);
        }
    }
}