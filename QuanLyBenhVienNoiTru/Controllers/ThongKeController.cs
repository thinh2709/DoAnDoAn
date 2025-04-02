using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBenhVienNoiTru.Models.Context;
using QuanLyBenhVienNoiTru.Models.Entities;
using QuanLyBenhVienNoiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyBenhVienNoiTru.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Bác sĩ")]
    public class ThongKeController : ControllerBase
    {
        private readonly QuanLyBenhVienNoiTru.Models.Context.ApplicationDbContext _context;

        public ThongKeController(QuanLyBenhVienNoiTru.Models.Context.ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ThongKe/TongQuan
        [HttpGet("TongQuan")]
        public async Task<ActionResult<object>> GetThongKeTongQuan()
        {
            // Số lượng bệnh nhân đang điều trị
            int soBenhNhanDangDieuTri = await _context.BenhNhan
                .CountAsync(b => b.NgayXuatVien == null);

            // Số lượng bệnh nhân đã xuất viện trong tháng này
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            int soBenhNhanXuatVienTrongThang = await _context.BenhNhan
                .CountAsync(b => b.NgayXuatVien >= firstDayOfMonth && b.NgayXuatVien <= lastDayOfMonth);

            // Tổng chi phí điều trị đã thanh toán trong tháng
            decimal tongDoanhThuThang = await _context.ChiPhiDieuTri
                .Where(c => c.DaThanhToan && c.NgayLap >= firstDayOfMonth && c.NgayLap <= lastDayOfMonth)
                .SumAsync(c => c.TongChiPhi);

            // Số lượng lịch thăm bệnh đã duyệt hôm nay
            int soLichThamBenhHomNay = await _context.LichThamBenh
                .CountAsync(l => l.ThoiGianTham.Date == DateTime.Today && l.TrangThai == "Đã duyệt");

            return new
            {
                SoBenhNhanDangDieuTri = soBenhNhanDangDieuTri,
                SoBenhNhanXuatVienTrongThang = soBenhNhanXuatVienTrongThang,
                TongDoanhThuThang = tongDoanhThuThang,
                SoLichThamBenhHomNay = soLichThamBenhHomNay
            };
        }

        // GET: api/ThongKe/BenhNhanTheoKhoa
        [HttpGet("BenhNhanTheoKhoa")]
        public async Task<ActionResult<object>> GetThongKeBenhNhanTheoKhoa()
        {
            var thongKe = await _context.Khoa
                .Select(k => new
                {
                    k.MaKhoa,
                    k.TenKhoa,
                    SoBenhNhanDangDieuTri = _context.BenhNhan.Count(b => b.MaKhoa == k.MaKhoa && b.NgayXuatVien == null),
                    TongSoBenhNhan = _context.BenhNhan.Count(b => b.MaKhoa == k.MaKhoa)
                })
                .ToListAsync();

            return thongKe;
        }

        // GET: api/ThongKe/DoanhThuTheoThang?nam=2023
        [HttpGet("DoanhThuTheoThang")]
        public async Task<ActionResult<object>> GetThongKeDoanhThuTheoThang(int nam = 0)
        {
            if (nam == 0)
            {
                nam = DateTime.Now.Year;
            }

            var doanhThuTheoThang = new List<object>();

            for (int thang = 1; thang <= 12; thang++)
            {
                var firstDay = new DateTime(nam, thang, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);

                decimal doanhThu = await _context.ChiPhiDieuTri
                    .Where(c => c.DaThanhToan && c.NgayLap >= firstDay && c.NgayLap <= lastDay)
                    .SumAsync(c => c.TongChiPhi);

                doanhThuTheoThang.Add(new
                {
                    Thang = thang,
                    DoanhThu = doanhThu
                });
            }

            return new
            {
                Nam = nam,
                DoanhThuTheoThang = doanhThuTheoThang
            };
        }

        // GET: api/ThongKe/HinhThucDieuTriPhoBien
        [HttpGet("HinhThucDieuTriPhoBien")]
        public async Task<ActionResult<object>> GetHinhThucDieuTriPhoBien()
        {
            var thongKe = await _context.DieuTriBenhNhan
                .GroupBy(d => d.MaDieuTri)
                .Select(g => new
                {
                    MaDieuTri = g.Key,
                    TenDieuTri = _context.HinhThucDieuTri
                        .Where(h => h.MaDieuTri == g.Key)
                        .Select(h => h.TenDieuTri)
                        .FirstOrDefault(),
                    SoLuot = g.Count()
                })
                .OrderByDescending(x => x.SoLuot)
                .Take(10)
                .ToListAsync();

            return thongKe;
        }

        // GET: api/ThongKe/BacSiHoatDong
        [HttpGet("BacSiHoatDong")]
        public async Task<ActionResult<object>> GetThongKeBacSiHoatDong()
        {
            var thangNay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            
            var thongKe = await _context.BacSi
                .Select(b => new
                {
                    b.MaBacSi,
                    b.HoTen,
                    b.ChuyenKhoa,
                    SoCaDieuTriThangNay = _context.DieuTriBenhNhan
                        .Count(d => d.MaBacSi == b.MaBacSi && d.NgayThucHien >= thangNay),
                    TongSoCaDieuTri = _context.DieuTriBenhNhan
                        .Count(d => d.MaBacSi == b.MaBacSi)
                })
                .OrderByDescending(x => x.SoCaDieuTriThangNay)
                .ToListAsync();

            return thongKe;
        }

        // GET: api/ThongKe/ThoiGianDieuTriTrungBinh
        [HttpGet("ThoiGianDieuTriTrungBinh")]
        public async Task<ActionResult<object>> GetThoiGianDieuTriTrungBinh()
        {
            // Chỉ tính cho bệnh nhân đã xuất viện
            var benhNhanDaXuatVien = await _context.BenhNhan
                .Where(b => b.NgayXuatVien != null)
                .ToListAsync();

            if (!benhNhanDaXuatVien.Any())
            {
                return new
                {
                    ThoiGianDieuTriTrungBinhNgay = 0
                };
            }

            // Tính thời gian điều trị trung bình (ngày)
            double tongSoNgayDieuTri = benhNhanDaXuatVien
                .Sum(b => (b.NgayXuatVien.Value - b.NgayNhapVien).TotalDays);

            double thoiGianTrungBinh = tongSoNgayDieuTri / benhNhanDaXuatVien.Count;

            // Phân tích theo khoa
            var thongKeTheoKhoa = await _context.Khoa
                .Select(k => new
                {
                    k.MaKhoa,
                    k.TenKhoa,
                    ThoiGianTrungBinh = _context.BenhNhan
                        .Where(b => b.MaKhoa == k.MaKhoa && b.NgayXuatVien != null)
                        .ToList()
                        .Select(b => (b.NgayXuatVien.Value - b.NgayNhapVien).TotalDays)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .ToListAsync();

            return new
            {
                ThoiGianDieuTriTrungBinhNgay = Math.Round(thoiGianTrungBinh, 1),
                ThongKeTheoKhoa = thongKeTheoKhoa
            };
        }

        // GET: api/ThongKe/TiLeBaoHiem
        [HttpGet("TiLeBaoHiem")]
        public async Task<ActionResult<object>> GetTiLeBaoHiem()
        {
            int tongSoBenhNhan = await _context.BenhNhan.CountAsync();
            
            if (tongSoBenhNhan == 0)
            {
                return new
                {
                    TongSoBenhNhan = 0,
                    SoBenhNhanCoBaoHiem = 0,
                    TiLeBaoHiem = 0
                };
            }

            int soBenhNhanCoBaoHiem = await _context.BenhNhan
                .CountAsync(b => b.BaoHiemYTe);

            double tiLeBaoHiem = (double)soBenhNhanCoBaoHiem / tongSoBenhNhan * 100;

            return new
            {
                TongSoBenhNhan = tongSoBenhNhan,
                SoBenhNhanCoBaoHiem = soBenhNhanCoBaoHiem,
                TiLeBaoHiem = Math.Round(tiLeBaoHiem, 2)
            };
        }
    }
}