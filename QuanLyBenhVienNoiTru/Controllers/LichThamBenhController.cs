using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using QuanLyBenhVienNoiTru.Models.Entities;
using QuanLyBenhVienNoiTru.Models.Context;

namespace QuanLyBenhVienNoiTru.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichThamBenhController : ControllerBase
    {
        private readonly QuanLyBenhVienNoiTru.Models.Context.ApplicationDbContext _context;

        public LichThamBenhController(QuanLyBenhVienNoiTru.Models.Context.ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/LichThamBenh
        [HttpGet]
        [Authorize(Roles = "Admin, Bác sĩ")]
        public async Task<ActionResult<IEnumerable<QuanLyBenhVienNoiTru.Models.Entities.LichThamBenh>>> GetLichThamBenh()
        {
            return await _context.LichThamBenh
                .Include(l => l.KhachThamBenh)
                .Include(l => l.BenhNhan)
                .ToListAsync();
        }

        // GET: api/LichThamBenh/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<QuanLyBenhVienNoiTru.Models.Entities.LichThamBenh>> GetLichThamBenh(int id)
        {
            var lichThamBenh = await _context.LichThamBenh
                .Include(l => l.KhachThamBenh)
                .Include(l => l.BenhNhan)
                .FirstOrDefaultAsync(l => l.MaLich == id);

            if (lichThamBenh == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập cho Khách
            if (User.IsInRole("Khách"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Forbid();
                }

                var userId = int.Parse(userIdClaim.Value);
                var khach = await _context.KhachThamBenh.FirstOrDefaultAsync(k => k.MaTaiKhoan == userId);
                
                if (khach == null || lichThamBenh.MaKhach != khach.MaKhach)
                {
                    return Forbid();
                }
            }

            return lichThamBenh;
        }

        // GET: api/LichThamBenh/BenhNhan/5
        [HttpGet("BenhNhan/{maBenhNhan}")]
        [Authorize(Roles = "Admin, Bác sĩ")]
        public async Task<ActionResult<IEnumerable<QuanLyBenhVienNoiTru.Models.Entities.LichThamBenh>>> GetLichThamBenhByBenhNhan(int maBenhNhan)
        {
            return await _context.LichThamBenh
                .Include(l => l.KhachThamBenh)
                .Include(l => l.BenhNhan)
                .Where(l => l.MaBenhNhan == maBenhNhan)
                .ToListAsync();
        }

        // GET: api/LichThamBenh/Khach
        [HttpGet("Khach")]
        [Authorize(Roles = "Khách")]
        public async Task<ActionResult<IEnumerable<QuanLyBenhVienNoiTru.Models.Entities.LichThamBenh>>> GetLichThamBenhByKhach()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Forbid();
            }

            var userId = int.Parse(userIdClaim.Value);
            var khach = await _context.KhachThamBenh.FirstOrDefaultAsync(k => k.MaTaiKhoan == userId);
            
            if (khach == null)
            {
                return NotFound("Không tìm thấy thông tin khách thăm bệnh");
            }

            return await _context.LichThamBenh
                .Include(l => l.KhachThamBenh)
                .Include(l => l.BenhNhan)
                .Where(l => l.MaKhach == khach.MaKhach)
                .ToListAsync();
        }

        // POST: api/LichThamBenh
        [HttpPost]
        [Authorize(Roles = "Khách")]
        public async Task<ActionResult<QuanLyBenhVienNoiTru.Models.Entities.LichThamBenh>> PostLichThamBenh(QuanLyBenhVienNoiTru.Models.Entities.LichThamBenh lichThamBenh)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Forbid();
            }

            var userId = int.Parse(userIdClaim.Value);
            var khach = await _context.KhachThamBenh.FirstOrDefaultAsync(k => k.MaTaiKhoan == userId);
            
            if (khach == null)
            {
                return NotFound("Không tìm thấy thông tin khách thăm bệnh");
            }

            // Kiểm tra bệnh nhân còn nằm viện không
            var benhNhan = await _context.BenhNhan.FindAsync(lichThamBenh.MaBenhNhan);
            if (benhNhan == null)
            {
                return NotFound("Không tìm thấy bệnh nhân");
            }

            if (benhNhan.NgayXuatVien != null)
            {
                return BadRequest("Bệnh nhân đã xuất viện, không thể đặt lịch thăm");
            }

            // Gán thông tin khách thăm
            lichThamBenh.MaKhach = khach.MaKhach;
            lichThamBenh.TrangThai = "Chờ duyệt";
            
            _context.LichThamBenh.Add(lichThamBenh);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLichThamBenh", new { id = lichThamBenh.MaLich }, lichThamBenh);
        }

        // PUT: api/LichThamBenh/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutLichThamBenh(int id, QuanLyBenhVienNoiTru.Models.Entities.LichThamBenh lichThamBenh)
        {
            if (id != lichThamBenh.MaLich)
            {
                return BadRequest();
            }

            var existingLich = await _context.LichThamBenh.FindAsync(id);
            if (existingLich == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập
            if (User.IsInRole("Khách"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Forbid();
                }

                var userId = int.Parse(userIdClaim.Value);
                var khach = await _context.QuanLyBenhVienNoiTru.Models.Entities.KhachThamBenh.FirstOrDefaultAsync(k => k.MaTaiKhoan == userId);
                
                if (khach == null || existingLich.MaKhach != khach.MaKhach)
                {
                    return Forbid();
                }

                // Khách chỉ được phép hủy lịch thăm
                if (lichThamBenh.TrangThai != "Hủy")
                {
                    return BadRequest("Bạn chỉ có quyền hủy lịch thăm");
                }

                existingLich.TrangThai = "Hủy";
            }
            else if (User.IsInRole("Admin") || User.IsInRole("Bác sĩ"))
            {
                // Admin và Bác sĩ có thể duyệt hoặc hủy lịch thăm
                existingLich.TrangThai = lichThamBenh.TrangThai;
            }
            else
            {
                return Forbid();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LichThamBenhExists(id))
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

        // DELETE: api/LichThamBenh/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLichThamBenh(int id)
        {
            var lichThamBenh = await _context.LichThamBenh.FindAsync(id);
            if (lichThamBenh == null)
            {
                return NotFound();
            }

            _context.LichThamBenh.Remove(lichThamBenh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LichThamBenhExists(int id)
        {
            return _context.LichThamBenh.Any(e => e.MaLich == id);
        }
    }
}