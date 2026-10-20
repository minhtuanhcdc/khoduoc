using KhoDuoc.Data;
using KhoDuoc.Models;
using KhoDuoc.DTOs;  // ← Thêm dòng này
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhoDuoc.Helpers;

namespace KhoDuoc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.VaiTro)
                .Include(u => u.DonVi)
                .Include(u => u.CoSo)
                .Select(u => new
                {
                    u.MaUser,
                    u.Username,
                    u.HoTen,
                    u.Email,
                    u.IsActive,
                    Role = u.VaiTro != null ? u.VaiTro.TenVaiTro : null,
                    DonVi = u.DonVi != null ? u.DonVi.TenDonVi : null,
                    CoSo = u.CoSo != null ? u.CoSo.TenCoSo : null
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.VaiTro)
                .Include(u => u.DonVi)
                .Include(u => u.CoSo)
                .FirstOrDefaultAsync(u => u.MaUser == id);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy user" });

            return Ok(new
            {
                user.MaUser,
                user.Username,
                user.HoTen,
                user.Email,
                user.IsActive,
                user.MaDonVi,
                user.MaCoSo,
                user.MaVaiTro,
                Role = user.VaiTro?.TenVaiTro,
                DonVi = user.DonVi?.TenDonVi,
                CoSo = user.CoSo?.TenCoSo
            });
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (existingUser != null)
                return BadRequest(new { message = "Username đã tồn tại" });

            var user = new User
            {
                Username = request.Username,
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                HoTen = request.HoTen,
                Email = request.Email,
                MaDonVi = request.MaDonVi,
                MaCoSo = request.MaCoSo,
                MaVaiTro = request.MaVaiTro,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo user thành công", userId = user.MaUser });
        }

        // PUT: api/users/{id}/change-password
        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy user" });

            if (!PasswordHelper.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Mật khẩu hiện tại không đúng" });

            user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }
    }
}