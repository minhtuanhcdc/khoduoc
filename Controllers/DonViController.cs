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
    public class DonViController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DonViController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.DonVis
                .Where(d => d.IsActive)
                .Select(d => new
                {
                    d.MaDonVi,
                    d.MaDonViCode,
                    d.TenDonVi,
                    d.DiaChi,
                    d.DienThoai,
                    d.Email
                })
                .ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var donVi = await _context.DonVis
                .FirstOrDefaultAsync(d => d.MaDonVi == id && d.IsActive);

            if (donVi == null)
                return NotFound();

            return Ok(donVi);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DonVi donVi)
        {
            donVi.IsActive = true;
            _context.DonVis.Add(donVi);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = donVi.MaDonVi }, donVi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DonVi donVi)
        {
            if (id != donVi.MaDonVi)
                return BadRequest();

            var existing = await _context.DonVis.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.MaDonViCode = donVi.MaDonViCode;
            existing.TenDonVi = donVi.TenDonVi;
            existing.DiaChi = donVi.DiaChi;
            existing.DienThoai = donVi.DienThoai;
            existing.Email = donVi.Email;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var donVi = await _context.DonVis.FindAsync(id);
            if (donVi == null)
                return NotFound();

            donVi.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully" });
        }

        [HttpGet("{id}/coso")]
        public async Task<IActionResult> GetCoSoByDonVi(int id)
        {
            var result = await _context.CoSos
                .Where(c => c.MaDonVi == id && c.IsActive)
                .Select(c => new
                {
                    c.MaCoSo,
                    c.MaCoSoCode,
                    c.TenCoSo,
                    c.DiaChi,
                    c.DienThoai,
                    c.LaCoSoChinh
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}