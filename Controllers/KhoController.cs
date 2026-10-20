using KhoDuoc.Data;
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
    public class KhoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public KhoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.Khos
                .Include(k => k.CoSo)
                .Select(k => new
                {
                    k.MaKho,
                    k.MaKhoCode,
                    k.TenKho,
                    k.DiaChi,
                    CoSo = k.CoSo != null ? k.CoSo.TenCoSo : null
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var kho = await _context.Khos
                .Include(k => k.CoSo)
                .FirstOrDefaultAsync(k => k.MaKho == id);

            if (kho == null)
                return NotFound();

            return Ok(kho);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Kho kho)
        {
            _context.Khos.Add(kho);
            await _context.SaveChangesAsync();
            return Ok(kho);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Kho kho)
        {
            if (id != kho.MaKho)
                return BadRequest();

            var existing = await _context.Khos.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.MaKhoCode = kho.MaKhoCode;
            existing.TenKho = kho.TenKho;
            existing.DiaChi = kho.DiaChi;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var kho = await _context.Khos.FindAsync(id);
            if (kho == null)
                return NotFound();

            _context.Khos.Remove(kho);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully" });
        }
    }
}