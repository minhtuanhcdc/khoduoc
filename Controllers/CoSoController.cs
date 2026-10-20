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
    public class CoSoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoSoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.CoSos
                .Include(c => c.DonVi)
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    c.MaCoSo,
                    c.MaCoSoCode,
                    c.TenCoSo,
                    c.DiaChi,
                    c.DienThoai,
                    c.LaCoSoChinh,
                    DonVi = c.DonVi != null ? c.DonVi.TenDonVi : null
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var coSo = await _context.CoSos
                .Include(c => c.DonVi)
                .FirstOrDefaultAsync(c => c.MaCoSo == id && c.IsActive);

            if (coSo == null)
                return NotFound();

            return Ok(coSo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CoSo coSo)
        {
            coSo.IsActive = true;
            _context.CoSos.Add(coSo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = coSo.MaCoSo }, coSo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CoSo coSo)
        {
            if (id != coSo.MaCoSo)
                return BadRequest();

            var existing = await _context.CoSos.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.MaCoSoCode = coSo.MaCoSoCode;
            existing.TenCoSo = coSo.TenCoSo;
            existing.DiaChi = coSo.DiaChi;
            existing.DienThoai = coSo.DienThoai;
            existing.LaCoSoChinh = coSo.LaCoSoChinh;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var coSo = await _context.CoSos.FindAsync(id);
            if (coSo == null)
                return NotFound();

            coSo.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully" });
        }

        [HttpGet("{id}/kho")]
        public async Task<IActionResult> GetKhoByCoSo(int id)
        {
            var result = await _context.Khos
                .Where(k => k.MaCoSo == id)
                .Select(k => new
                {
                    k.MaKho,
                    k.MaKhoCode,
                    k.TenKho,
                    k.DiaChi
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}