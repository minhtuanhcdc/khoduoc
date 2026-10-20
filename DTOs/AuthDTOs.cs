namespace KhoDuoc.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public int? MaDonVi { get; set; }
        public int? MaCoSo { get; set; }
        public int? MaVaiTro { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int? MaDonVi { get; set; }
        public string? TenDonVi { get; set; }
        public int? MaCoSo { get; set; }
        public string? TenCoSo { get; set; }
    }
}
