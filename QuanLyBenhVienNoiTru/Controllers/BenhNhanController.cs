using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBenhVienNoiTru.Models.ViewModels;
using QuanLyBenhVienNoiTru.Repositories;
using QuanLyBenhVienNoiTru.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyBenhVienNoiTru.Controllers
{
    [Authorize]
    public class BenhNhanController : Controller
    {
        private readonly IBenhNhanService _benhNhanService;
        private readonly IGenericRepository<Models.Entities.Khoa> _khoaRepository;

        public BenhNhanController(IBenhNhanService benhNhanService, IGenericRepository<Models.Entities.Khoa> khoaRepository)
        {
            _benhNhanService = benhNhanService;
            _khoaRepository = khoaRepository;
        }

        public async Task<IActionResult> Index()
        {
            var benhNhans = await _benhNhanService.GetAllBenhNhanAsync();
            return View(benhNhans);
        }

        public async Task<IActionResult> Details(int id)
        {
            var benhNhan = await _benhNhanService.GetBenhNhanByIdAsync(id);
            if (benhNhan == null)
            {
                return NotFound();
            }
            return View(benhNhan);
        }

        public async Task<IActionResult> Create()
        {
            var khoas = await _khoaRepository.GetAllAsync();
            ViewBag.Khoas = new SelectList(khoas, "MaKhoa", "TenKhoa");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BenhNhanViewModel benhNhanVM)
        {
            if (ModelState.IsValid)
            {
                await _benhNhanService.AddBenhNhanAsync(benhNhanVM);
                return RedirectToAction(nameof(Index));
            }

            var khoas = await _khoaRepository.GetAllAsync();
            ViewBag.Khoas = new SelectList(khoas, "MaKhoa", "TenKhoa");
            return View(benhNhanVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var benhNhan = await _benhNhanService.GetBenhNhanByIdAsync(id);
            if (benhNhan == null)
            {
                return NotFound();
            }

            var benhNhanVM = new BenhNhanViewModel
            {
                MaBenhNhan = benhNhan.MaBenhNhan,
                HoTen = benhNhan.HoTen,
                NgaySinh = benhNhan.NgaySinh,
                GioiTinh = benhNhan.GioiTinh,
                SoDienThoai = benhNhan.SoDienThoai,
                DiaChi = benhNhan.DiaChi,
                BaoHiemYTe = benhNhan.BaoHiemYTe,
                NgayNhapVien = benhNhan.NgayNhapVien,
                NgayXuatVien = benhNhan.NgayXuatVien,
                MaKhoa = benhNhan.MaKhoa
            };

            var khoas = await _khoaRepository.GetAllAsync();
            ViewBag.Khoas = new SelectList(khoas, "MaKhoa", "TenKhoa");
            return View(benhNhanVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BenhNhanViewModel benhNhanVM)
        {
            if (ModelState.IsValid)
            {
                await _benhNhanService.UpdateBenhNhanAsync(benhNhanVM);
                return RedirectToAction(nameof(Index));
            }

            var khoas = await _khoaRepository.GetAllAsync();
            ViewBag.Khoas = new SelectList(khoas, "MaKhoa", "TenKhoa");
            return View(benhNhanVM);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var benhNhan = await _benhNhanService.GetBenhNhanByIdAsync(id);
            if (benhNhan == null)
            {
                return NotFound();
            }
            return View(benhNhan);
        }

	[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _benhNhanService.DeleteBenhNhanAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}