using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuanLyBenhVienNoiTru.Models;
using QuanLyBenhVienNoiTru.Data;

namespace QuanLyBenhVienNoiTru.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachThamBenhController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KhachThamBenhController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/KhachThamBenh
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachThamBenh>>> GetKhachThamBenh()
        {
            return await _context.KhachThamBenh.ToListAsync();
        }

        // GET: api/KhachThamBenh/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KhachThamBenh>> GetKhachThamBenh(int id)
        {
            var khachThamBenh = await _context.KhachThamBenh.FindAsync(id);

            if (khachThamBenh == null)
            {
                return NotFound();
            }

            return khachThamBenh;
        }

        // GET: api/KhachThamBenh/TaiKhoan/5
        [HttpGet("TaiKhoan/{id}")]
        public async Task<ActionResult<KhachThamBenh>> GetKhachThamBenhByTaiKhoan(int id)
        {
            var khachThamBenh = await _context.KhachThamBenh
                .FirstOrDefaultAsync(k => k.MaTaiKhoan == id);

            if (khachThamBenh == null)
            {
                return NotFound();
            }

            return khachThamBenh;
        }

        // POST: api/KhachThamBenh
        [HttpPost]
        public async Task<ActionResult<KhachThamBenh>> PostKhachThamBenh(KhachThamBenh khachThamBenh)
        {
            // Kiểm tra nếu tài khoản đã tồn tại
            if (khachThamBenh.MaTaiKhoan != null)
            {
                var taiKhoanExists = await _context.TaiKhoan.FindAsync(khachThamBenh.MaTaiKhoan);
                if (taiKhoanExists == null)
                {
                    return BadRequest("Tài khoản không tồn tại");
                }
            }

            _context.KhachThamBenh.Add(khachThamBenh);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKhachThamBenh", new { id = khachThamBenh.MaKhach }, khachThamBenh);
        }

        // POST: api/KhachThamBenh/DangKy
        [HttpPost("DangKy")]
        public async Task<ActionResult<KhachThamBenh>> DangKyKhachThamBenh(DangKyKhachDto dangKyDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Tạo tài khoản mới
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = dangKyDto.TenDangNhap,
                    MatKhau = dangKyDto.MatKhau, // Nên mã hóa mật khẩu trong thực tế
                    VaiTro = "Khách"
                };

                _context.TaiKhoan.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Tạo thông tin khách thăm bệnh
                var khachThamBenh = new KhachThamBenh
                {
                    HoTen = dangKyDto.HoTen,
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    SoDienThoai = dangKyDto.SoDienThoai,
                    MoiQuanHe = dangKyDto.MoiQuanHe
                };

                _context.KhachThamBenh.Add(khachThamBenh);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction("GetKhachThamBenh", new { id = khachThamBenh.MaKhach }, khachThamBenh);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return BadRequest("Đăng ký không thành công. Vui lòng thử lại.");
            }
        }

        // PUT: api/KhachThamBenh/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKhachThamBenh(int id, KhachThamBenh khachThamBenh)
        {
            if (id != khachThamBenh.MaKhach)
            {
                return BadRequest();
            }

            _context.Entry(khachThamBenh).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KhachThamBenhExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/KhachThamBenh/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKhachThamBenh(int id)
        {
            var khachThamBenh = await _context.KhachThamBenh.FindAsync(id);
            if (khachThamBenh == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu khách có lịch thăm bệnh
            var hasLichTham = await _context.LichThamBenh.AnyAsync(l => l.MaKhach == id);
            if (hasLichTham)
            {
                return BadRequest("Không thể xóa vì khách đã có lịch thăm bệnh");
            }

            _context.KhachThamBenh.Remove(khachThamBenh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KhachThamBenhExists(int id)
        {
            return _context.KhachThamBenh.Any(e => e.MaKhach == id);
        }
    }

    // DTO cho đăng ký khách thăm bệnh
    public class DangKyKhachDto
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string MoiQuanHe { get; set; }
    }
}