using KhoDuoc.Data;
using KhoDuoc.DTOs;
using KhoDuoc.Helpers;
using KhoDuoc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KhoDuoc.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> Login(LoginRequest request);
        Task<User?> Register(RegisterRequest request);
        Task<AuthResponse?> RefreshToken(string refreshToken);
        Task<bool> ChangePassword(int userId, ChangePasswordRequest request);
        Task<UserDto?> GetCurrentUser(int userId);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse?> Login(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.VaiTro)
                .Include(u => u.DonVi)
                .Include(u => u.CoSo)
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

            if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
                return null;

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.MaUser,
                    Username = user.Username,
                    HoTen = user.HoTen,
                    Email = user.Email,
                    Role = user.VaiTro?.TenVaiTro,
                    MaDonVi = user.MaDonVi,
                    TenDonVi = user.DonVi?.TenDonVi,
                    MaCoSo = user.MaCoSo,
                    TenCoSo = user.CoSo?.TenCoSo
                }
            };
        }

        public async Task<User?> Register(RegisterRequest request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (existingUser != null)
                return null;

            var user = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                HoTen = request.HoTen,
                Email = request.Email,
                MaDonVi = request.MaDonVi,
                MaCoSo = request.MaCoSo,
                MaVaiTro = request.MaVaiTro,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<AuthResponse?> RefreshToken(string refreshToken)
        {
            // TODO: Implement refresh token logic
            await Task.CompletedTask;
            return null;
        }

        public async Task<bool> ChangePassword(int userId, ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.PasswordHash != HashPassword(request.CurrentPassword))
                return false;

            user.PasswordHash = HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDto?> GetCurrentUser(int userId)
        {
            var user = await _context.Users
                .Include(u => u.VaiTro)
                .Include(u => u.DonVi)
                .Include(u => u.CoSo)
                .FirstOrDefaultAsync(u => u.MaUser == userId);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.MaUser,
                Username = user.Username,
                HoTen = user.HoTen,
                Email = user.Email,
                Role = user.VaiTro?.TenVaiTro,
                MaDonVi = user.MaDonVi,
                TenDonVi = user.DonVi?.TenDonVi,
                MaCoSo = user.MaCoSo,
                TenCoSo = user.CoSo?.TenCoSo
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"]
    ?? throw new Exception("JWT Key is missing in configuration");

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaUser.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.VaiTro?.TenVaiTro ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToHexString(bytes);
            }
        }
    }
}