using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBenhVienNoiTru.Models.Entities;
using QuanLyBenhVienNoiTru.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyBenhVienNoiTru.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChiPhiController : ControllerBase
    {
        private readonly QuanLyBenhVienNoiTru.Models.Context.ApplicationDbContext _context;

        public ChiPhiController(QuanLyBenhVienNoiTru.Models.Context.ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ChiPhi
        [HttpGet]
        [Authorize(Roles = "Admin, Bác sĩ")]
        public async Task<ActionResult<IEnumerable<ChiPhiDieuTri>>> GetChiPhi()
        {
            return await _context.ChiPhiDieuTri
                .Include(c => c.BenhNhan)
                .ToListAsync();
        }

        // GET: api/ChiPhi/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Bác sĩ")]
        public async Task<ActionResult<ChiPhiDieuTri>> GetChiPhi(int id)
        {
            var chiPhi = await _context.ChiPhiDieuTri
                .Include(c => c.BenhNhan)
                .FirstOrDefaultAsync(c => c.MaChiPhi == id);

            if (chiPhi == null)
            {
                return NotFound();
            }

            return chiPhi;
        }

        // GET: api/ChiPhi/BenhNhan/5
        [HttpGet("BenhNhan/{maBenhNhan}")]
        [Authorize(Roles = "Admin, Bác sĩ")]
        public async Task<ActionResult<IEnumerable<ChiPhiDieuTri>>> GetChiPhiByBenhNhan(int maBenhNhan)
        {
            return await _context.ChiPhiDieuTri
                .Include(c => c.BenhNhan)
                .Where(c => c.MaBenhNhan == maBenhNhan)
                .ToListAsync();
        }

        // GET: api/ChiPhi/ChiTiet/5
        [HttpGet("ChiTiet/{maBenhNhan}")]
        [Authorize(Roles = "Admin, Bác sĩ")]
        public async Task<ActionResult<object>> GetChiTietChiPhi(int maBenhNhan)
        {
            var benhNhan = await _context.BenhNhan.FindAsync(maBenhNhan);
            if (benhNhan == null)
            {
                return NotFound("Không tìm thấy bệnh nhân");
            }

            var dieuTri = await _context.DieuTriBenhNhan
                .Include(d => d.HinhThucDieuTri)
                .Include(d => d.BacSi)
                .Where(d => d.MaBenhNhan == maBenhNhan)
                .ToListAsync();

            var chiTiet = dieuTri.Select(d => new
            {
                NgayThucHien = d.NgayThucHien,
                TenDieuTri = d.HinhThucDieuTri.TenDieuTri,
                BacSiThucHien = d.BacSi.HoTen,
                ChiPhi = d.HinhThucDieuTri.ChiPhi,
                KetQua = d.KetQua
            }).ToList();

            var tongChiPhi = dieuTri.Sum(d => d.HinhThucDieuTri.ChiPhi);
            var giamTruBaoHiem = benhNhan.BaoHiemYTe ? tongChiPhi * 0.8m : 0;
            var chiPhiPhaiTra = tongChiPhi - giamTruBaoHiem;

            return new
            {
                ThongTinBenhNhan = new
                {
                    benhNhan.MaBenhNhan,
                    benhNhan.HoTen,
                    benhNhan.NgaySinh,
                    benhNhan.NgayNhapVien,
                    benhNhan.NgayXuatVien,
                    benhNhan.BaoHiemYTe
                },
                ChiTietDieuTri = chiTiet,
                TongHop = new
                {
                    TongChiPhi = tongChiPhi,
                    GiamTruBaoHiem = giamTruBaoHiem,
                    ChiPhiPhaiTra = chiPhiPhaiTra
                }
            };
        }

        // POST: api/ChiPhi
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChiPhiDieuTri>> PostChiPhi(ChiPhiDieuTri chiPhi)
        {
            var benhNhan = await _context.BenhNhan.FindAsync(chiPhi.MaBenhNhan);
            if (benhNhan == null)
            {
                return NotFound("Không tìm thấy bệnh nhân");
            }

            // Tự động tính toán chi phí
            var dieuTri = await _context.DieuTriBenhNhan
                .Include(d => d.HinhThucDieuTri)
                .Where(d => d.MaBenhNhan == chiPhi.MaBenhNhan)
                .ToListAsync();

            decimal tongChiPhiDieuTri = dieuTri.Sum(d => d.HinhThucDieuTri.ChiPhi);
            
            // Giảm trừ 80% nếu có bảo hiểm y tế
            if (benhNhan.BaoHiemYTe)
            {
                tongChiPhiDieuTri = tongChiPhiDieuTri * 0.2m; // Bệnh nhân chỉ trả 20%
            }

            // Gán thông tin chi phí
            chiPhi.TongChiPhi = tongChiPhiDieuTri;
            chiPhi.NgayLap = DateTime.Now;
            
            _context.ChiPhiDieuTri.Add(chiPhi);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChiPhi", new { id = chiPhi.MaChiPhi }, chiPhi);
        }

        // PUT: api/ChiPhi/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutChiPhi(int id, ChiPhiDieuTri chiPhi)
        {
            if (id != chiPhi.MaChiPhi)
            {
                return BadRequest();
            }

            var existingChiPhi = await _context.ChiPhiDieuTri.FindAsync(id);
            if (existingChiPhi == null)
            {
                return NotFound();
            }

            // Chỉ cho phép cập nhật trạng thái thanh toán
            existingChiPhi.DaThanhToan = chiPhi.DaThanhToan;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChiPhiExists(id))
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

        // DELETE: api/ChiPhi/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChiPhi(int id)
        {
            var chiPhi = await _context.ChiPhiDieuTri.FindAsync(id);
            if (chiPhi == null)
            {
                return NotFound();
            }

            _context.ChiPhiDieuTri.Remove(chiPhi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChiPhiExists(int id)
        {
            return _context.ChiPhiDieuTri.Any(e => e.MaChiPhi == id);
        }
    }
}