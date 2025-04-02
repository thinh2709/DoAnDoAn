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
    public class KhoaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KhoaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Khoa
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Khoa>>> GetKhoa()
        {
            return await _context.Khoa.ToListAsync();
        }

        // GET: api/Khoa/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Khoa>> GetKhoa(int id)
        {
            var khoa = await _context.Khoa.FindAsync(id);

            if (khoa == null)
            {
                return NotFound();
            }

            return khoa;
        }

        // GET: api/Khoa/5/BenhNhan
        [HttpGet("{id}/BenhNhan")]
        public async Task<ActionResult<IEnumerable<BenhNhan>>> GetBenhNhanByKhoa(int id)
        {
            var khoa = await _context.Khoa.FindAsync(id);

            if (khoa == null)
            {
                return NotFound();
            }

            return await _context.BenhNhan
                .Where(bn => bn.MaKhoa == id && bn.NgayXuatVien == null)
                .ToListAsync();
        }

        // POST: api/Khoa
        [HttpPost]
        public async Task<ActionResult<Khoa>> PostKhoa(Khoa khoa)
        {
            _context.Khoa.Add(khoa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKhoa", new { id = khoa.MaKhoa }, khoa);
        }

        // PUT: api/Khoa/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKhoa(int id, Khoa khoa)
        {
            if (id != khoa.MaKhoa)
            {
                return BadRequest();
            }

            _context.Entry(khoa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KhoaExists(id))
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

        // DELETE: api/Khoa/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKhoa(int id)
        {
            var khoa = await _context.Khoa.FindAsync(id);
            if (khoa == null)
            {
                return NotFound();
            }

            // Check if there are any patients in this department
            var hasBenhNhan = await _context.BenhNhan.AnyAsync(bn => bn.MaKhoa == id);
            if (hasBenhNhan)
            {
                return BadRequest("Không thể xóa khoa này vì có bệnh nhân đang điều trị");
            }

            _context.Khoa.Remove(khoa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KhoaExists(int id)
        {
            return _context.Khoa.Any(e => e.MaKhoa == id);
        }
    }
}