using Microsoft.EntityFrameworkCore;
using QuanLyBenhVienNoiTru.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyBenhVienNoiTru.Models.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<BacSi> BacSis { get; set; }
        public DbSet<BenhNhan> BenhNhans { get; set; }
        public DbSet<Khoa> Khoas { get; set; }
        public DbSet<HinhThucDieuTri> HinhThucDieuTris { get; set; }
        public DbSet<DieuTriBenhNhan> DieuTriBenhNhans { get; set; }
        public DbSet<KhachThamBenh> KhachThamBenhs { get; set; }
        public DbSet<LichThamBenh> LichThamBenhs { get; set; }
        public DbSet<ChiPhiDieuTri> ChiPhiDieuTris { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình quan hệ giữa các bảng
            // TaiKhoan - BacSi
            modelBuilder.Entity<BacSi>()
                .HasOne(b => b.TaiKhoan)
                .WithOne(t => t.BacSi)
                .HasForeignKey<BacSi>(b => b.MaTaiKhoan);

            // TaiKhoan - KhachThamBenh
            modelBuilder.Entity<KhachThamBenh>()
                .HasOne(k => k.TaiKhoan)
                .WithOne(t => t.KhachThamBenh)
                .HasForeignKey<KhachThamBenh>(k => k.MaTaiKhoan);

            // BenhNhan - Khoa
            modelBuilder.Entity<BenhNhan>()
                .HasOne(b => b.Khoa)
                .WithMany(k => k.BenhNhans)
                .HasForeignKey(b => b.MaKhoa);

            // BenhNhan - DieuTriBenhNhan
            modelBuilder.Entity<DieuTriBenhNhan>()
                .HasOne(d => d.BenhNhan)
                .WithMany(b => b.DieuTriBenhNhans)
                .HasForeignKey(d => d.MaBenhNhan);

            // HinhThucDieuTri - DieuTriBenhNhan
            modelBuilder.Entity<DieuTriBenhNhan>()
                .HasOne(d => d.HinhThucDieuTri)
                .WithMany(h => h.DieuTriBenhNhans)
                .HasForeignKey(d => d.MaDieuTri);

            // BacSi - DieuTriBenhNhan
            modelBuilder.Entity<DieuTriBenhNhan>()
                .HasOne(d => d.BacSi)
                .WithMany(b => b.DieuTriBenhNhans)
                .HasForeignKey(d => d.MaBacSi);

            // KhachThamBenh - LichThamBenh
            modelBuilder.Entity<LichThamBenh>()
                .HasOne(l => l.KhachThamBenh)
                .WithMany(k => k.LichThamBenhs)
                .HasForeignKey(l => l.MaKhach);

            // BenhNhan - LichThamBenh
            modelBuilder.Entity<LichThamBenh>()
                .HasOne(l => l.BenhNhan)
                .WithMany(b => b.LichThamBenhs)
                .HasForeignKey(l => l.MaBenhNhan);

            // BenhNhan - ChiPhiDieuTri
            modelBuilder.Entity<ChiPhiDieuTri>()
                .HasOne(c => c.BenhNhan)
                .WithMany(b => b.ChiPhiDieuTris)
                .HasForeignKey(c => c.MaBenhNhan);
        }
    }
}