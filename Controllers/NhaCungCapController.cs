using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhoDuoc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NhaCungCapController : ControllerBase
    {
        private readonly AppDbContext _context;
        public NhaCungCapController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.NhaCungCaps
                .Select(d => new
                {
                    d.MaNCC,
                    d.TenNCC,
                    d.DiaChi,
                    d.DienThoai,
                    d.Email
                })
                .ToListAsync();
            return Ok(result);
        }

        // Lấy nhà cung cấp theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var nhaCungCap = await _context.NhaCungCaps
                .Where(d => d.MaNCC == id)
                .Select(d => new
                {
                    d.MaNCC,
                    d.TenNCC,
                    d.DiaChi,
                    d.DienThoai,
                    d.Email
                })
                .FirstOrDefaultAsync();

            if (nhaCungCap == null)
                return NotFound(new { message = "Không tìm thấy nhà cung cấp với ID: " + id });

            return Ok(nhaCungCap);
        }

        // Tìm kiếm nhà cung cấp theo tên
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm" });

            var result = await _context.NhaCungCaps
                .Where(d => d.TenNCC != null && (d.TenNCC.Contains(keyword) ||
                           (d.Email != null && d.Email.Contains(keyword)) ||
                           (d.DienThoai != null && d.DienThoai.Contains(keyword))))
                .Select(d => new
                {
                    d.MaNCC,
                    d.TenNCC,
                    d.DiaChi,
                    d.DienThoai,
                    d.Email
                })
                .ToListAsync();

            return Ok(result);
        }

        // Thêm mới nhà cung cấp
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NhaCungCap model)
        {
            if (model == null)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra trùng email hoặc số điện thoại
            var exists = await _context.NhaCungCaps
                .AnyAsync(d => (d.Email != null && d.Email == model.Email) ||
                               (d.DienThoai != null && d.DienThoai == model.DienThoai));

            if (exists)
                return Conflict(new { message = "Email hoặc số điện thoại đã tồn tại" });

            model.MaNCC = 0; // Đảm bảo ID tự động tăng
            _context.NhaCungCaps.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.MaNCC }, new
            {
                model.MaNCC,
                model.TenNCC,
                model.DiaChi,
                model.DienThoai,
                model.Email
            });
        }

        // Cập nhật thông tin nhà cung cấp
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] NhaCungCap model)
        {
            if (model == null)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            if (id != model.MaNCC)
                return BadRequest(new { message = "ID không khớp" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingNCC = await _context.NhaCungCaps.FindAsync(id);
            if (existingNCC == null)
                return NotFound(new { message = "Không tìm thấy nhà cung cấp với ID: " + id });

            // Kiểm tra trùng email/sdt với nhà cung cấp khác
            var duplicate = await _context.NhaCungCaps
                .AnyAsync(d => ((d.Email != null && d.Email == model.Email) ||
                                (d.DienThoai != null && d.DienThoai == model.DienThoai)) &&
                                d.MaNCC != id);

            if (duplicate)
                return Conflict(new { message = "Email hoặc số điện thoại đã được sử dụng bởi nhà cung cấp khác" });

            // Cập nhật thông tin
            existingNCC.TenNCC = model.TenNCC;
            existingNCC.DiaChi = model.DiaChi;
            existingNCC.DienThoai = model.DienThoai;
            existingNCC.Email = model.Email;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                existingNCC.MaNCC,
                existingNCC.TenNCC,
                existingNCC.DiaChi,
                existingNCC.DienThoai,
                existingNCC.Email
            });
        }

        // Xóa nhà cung cấp
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap == null)
                return NotFound(new { message = "Không tìm thấy nhà cung cấp với ID: " + id });

            // Kiểm tra xem có ràng buộc khóa ngoại với bảng khác không
            var hasRelatedData = await _context.PhieuNhaps.AnyAsync(p => p.MaNCC == id);
            if (hasRelatedData)
                return BadRequest(new { message = "Không thể xóa nhà cung cấp vì đã có phiếu nhập liên quan" });

            _context.NhaCungCaps.Remove(nhaCungCap);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa nhà cung cấp thành công" });
        }

        // Lấy danh sách nhà cung cấp có phân trang
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = await _context.NhaCungCaps.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await _context.NhaCungCaps
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    d.MaNCC,
                    d.TenNCC,
                    d.DiaChi,
                    d.DienThoai,
                    d.Email
                })
                .ToListAsync();

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items
            });
        }

        // Lấy danh sách nhà cung cấp có sắp xếp
        [HttpGet("sort")]
        public async Task<IActionResult> GetSorted([FromQuery] string sortBy = "TenNCC", [FromQuery] bool ascending = true)
        {
            var query = _context.NhaCungCaps.AsQueryable();

            // Áp dụng sắp xếp động
            query = (sortBy?.ToLower() ?? "tenncc") switch
            {
                "mancc" => ascending ? query.OrderBy(d => d.MaNCC) : query.OrderByDescending(d => d.MaNCC),
                "tenncc" => ascending ? query.OrderBy(d => d.TenNCC ?? "") : query.OrderByDescending(d => d.TenNCC ?? ""),
                "email" => ascending ? query.OrderBy(d => d.Email ?? "") : query.OrderByDescending(d => d.Email ?? ""),
                "dienthoai" => ascending ? query.OrderBy(d => d.DienThoai ?? "") : query.OrderByDescending(d => d.DienThoai ?? ""),
                _ => ascending ? query.OrderBy(d => d.TenNCC ?? "") : query.OrderByDescending(d => d.TenNCC ?? "")
            };

            var result = await query
                .Select(d => new
                {
                    d.MaNCC,
                    d.TenNCC,
                    d.DiaChi,
                    d.DienThoai,
                    d.Email
                })
                .ToListAsync();

            return Ok(result);
        }

        // Thống kê số lượng nhà cung cấp
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var totalCount = await _context.NhaCungCaps.CountAsync();
            var activeCount = totalCount; // Có thể thêm trường IsActive nếu cần

            return Ok(new
            {
                TotalSuppliers = totalCount,
                ActiveSuppliers = activeCount,
                LastUpdated = DateTime.UtcNow
            });
        }
    }
}