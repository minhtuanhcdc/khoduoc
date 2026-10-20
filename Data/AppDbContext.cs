using Microsoft.EntityFrameworkCore;
using KhoDuoc.Models;  // ← THÊM DÒNG NÀY

namespace KhoDuoc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Các DbSet
        public DbSet<User> Users { get; set; }          // User từ namespace KhoDuoc.Models
        public DbSet<DonVi> DonVis { get; set; }
        public DbSet<CoSo> CoSos { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<Kho> Khos { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public DbSet<Thuoc> Thuocs { get; set; }
        public DbSet<LoThuoc> LoThuocs { get; set; }
        public DbSet<PhieuNhap> PhieuNhaps { get; set; }
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }
        public DbSet<PhieuXuat> PhieuXuats { get; set; }
        public DbSet<ChiTietPhieuXuat> ChiTietPhieuXuats { get; set; }
        public DbSet<YeuCauThuoc> YeuCauThuocs { get; set; }
        public DbSet<ChiTietYeuCauThuoc> ChiTietYeuCauThuocs { get; set; }
        public DbSet<TonKho> TonKhos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DonVi>()
                .HasIndex(d => d.MaDonViCode)
                .IsUnique();

            modelBuilder.Entity<CoSo>()
                .HasIndex(c => c.MaCoSoCode)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Kho>()
                .HasIndex(k => k.MaKhoCode)
                .IsUnique();

            modelBuilder.Entity<Thuoc>()
                .HasIndex(t => t.MaThuocCode)
                .IsUnique();

            modelBuilder.Entity<TonKho>()
                .HasIndex(t => new { t.MaKho, t.MaLo })
                .IsUnique();
            //Trigger
            modelBuilder.Entity<PhieuNhap>()
                .ToTable(tb => tb.HasTrigger("AnyTrigger"));

            modelBuilder.Entity<ChiTietPhieuNhap>()
                .ToTable(tb => tb.HasTrigger("AnyTrigger"));

            modelBuilder.Entity<LoThuoc>()
                .ToTable(tb => tb.HasTrigger("AnyTrigger"));

            modelBuilder.Entity<PhieuXuat>()
                .ToTable(tb => tb.HasTrigger("AnyTrigger"));

            modelBuilder.Entity<ChiTietPhieuXuat>()
                .ToTable(tb => tb.HasTrigger("AnyTrigger"));

            modelBuilder.Entity<TonKho>()
                .ToTable(tb => tb.HasTrigger("AnyTrigger"));
        }
    }
}