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
    public class DieuTriController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DieuTriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DieuTri
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HinhThucDieuTri>>> GetHinhThucDieuTri()
        {
            return await _context.HinhThucDieuTri.ToListAsync();
        }

        // GET: api/DieuTri/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HinhThucDieuTri>> GetHinhThucDieuTri(int id)
        {
            var hinhThucDieuTri = await _context.HinhThucDieuTri.FindAsync(id);

            if (hinhThucDieuTri == null)
            {
                return NotFound();
            }

            return hinhThucDieuTri;
        }

        // POST: api/DieuTri
        [HttpPost]
        public async Task<ActionResult<HinhThucDieuTri>> PostHinhThucDieuTri(HinhThucDieuTri hinhThucDieuTri)
        {
            _context.HinhThucDieuTri.Add(hinhThucDieuTri);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHinhThucDieuTri", new { id = hinhThucDieuTri.MaDieuTri }, hinhThucDieuTri);
        }

        // PUT: api/DieuTri/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHinhThucDieuTri(int id, HinhThucDieuTri hinhThucDieuTri)
        {
            if (id != hinhThucDieuTri.MaDieuTri)
            {
                return BadRequest();
            }

            _context.Entry(hinhThucDieuTri).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HinhThucDieuTriExists(id))
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

        // DELETE: api/DieuTri/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHinhThucDieuTri(int id)
        {
            var hinhThucDieuTri = await _context.HinhThucDieuTri.FindAsync(id);
            if (hinhThucDieuTri == null)
            {
                return NotFound();
            }

            // Check if this treatment is being used
            var isInUse = await _context.DieuTriBenhNhan.AnyAsync(dt => dt.MaDieuTri == id);
            if (isInUse)
            {
                return BadRequest("Không thể xóa hình thức điều trị này vì đang được sử dụng");
            }

            _context.HinhThucDieuTri.Remove(hinhThucDieuTri);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/DieuTri/BenhNhan/5
        [HttpGet("BenhNhan/{id}")]
        public async Task<ActionResult<IEnumerable<DieuTriBenhNhan>>> GetDieuTriCuaBenhNhan(int id)
        {
            var benhNhan = await _context.BenhNhan.FindAsync(id);
            
            if (benhNhan == null)
            {
                return NotFound();
            }

            return await _context.DieuTriBenhNhan
                .Where(dt => dt.MaBenhNhan == id)
                .Include(dt => dt.HinhThucDieuTri)
                .Include(dt => dt.BacSi)
                .OrderByDescending(dt => dt.NgayThucHien)
                .ToListAsync();
        }

        // POST: api/DieuTri/BenhNhan
        [HttpPost("BenhNhan")]
        public async Task<ActionResult<DieuTriBenhNhan>> ThemDieuTriChoBenhNhan(DieuTriBenhNhan dieuTri)
        {
            var benhNhan = await _context.BenhNhan.FindAsync(dieuTri.MaBenhNhan);
            var hinhThucDieuTri = await _context.HinhThucDieuTri.FindAsync(dieuTri.MaDieuTri);
            var bacSi = await _context.BacSi.FindAsync(dieuTri.MaBacSi);
            
            if (benhNhan == null || hinhThucDieuTri == null || bacSi == null)
            {
                return BadRequest("Thông tin không hợp lệ");
            }

            // Set ngày thực hiện là ngày hiện tại nếu không được cung cấp
            if (dieuTri.NgayThucHien == default)
            {
                dieuTri.NgayThucHien = DateTime.Now;
            }

            _context.DieuTriBenhNhan.Add(dieuTri);
            await _context.SaveChangesAsync();

            // Cập nhật chi phí điều trị
            var chiPhi = await _context.ChiPhiDieuTri
                .Where(cp => cp.MaBenhNhan == dieuTri.MaBenhNhan && !cp.DaThanhToan)
                .FirstOrDefaultAsync();

            if (chiPhi == null)
            {
                chiPhi = new ChiPhiDieuTri
                {
                    MaBenhNhan = dieuTri.MaBenhNhan,
                    TongChiPhi = hinhThucDieuTri.ChiPhi,
                    DaThanhToan = false,
                    NgayLap = DateTime.Now
                };
                _context.ChiPhiDieuTri.Add(chiPhi);
            }
            else
            {
                chiPhi.TongChiPhi += hinhThucDieuTri.ChiPhi;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDieuTriCuaBenhNhan", new { id = dieuTri.MaBenhNhan }, dieuTri);
        }

        // PUT: api/DieuTri/BenhNhan/5
        [HttpPut("BenhNhan/{id}")]
        public async Task<IActionResult> CapNhatDieuTriBenhNhan(int id, DieuTriBenhNhan dieuTri)
        {
            if (id != dieuTri.MaDieuTriBenhNhan)
            {
                return BadRequest();
            }

            _context.Entry(dieuTri).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DieuTriBenhNhanExists(id))
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

        private bool HinhThucDieuTriExists(int id)
        {
            return _context.HinhThucDieuTri.Any(e => e.MaDieuTri == id);
        }

        private bool DieuTriBenhNhanExists(int id)
        {
            return _context.DieuTriBenhNhan.Any(e => e.MaDieuTriBenhNhan == id);
        }
    }
}