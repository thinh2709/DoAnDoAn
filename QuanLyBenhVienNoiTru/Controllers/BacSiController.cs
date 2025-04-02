using System;
using System.Linq;
using System.Net;
using QuanLyBenhVienNoiTru.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBenhVienNoiTru.Models.Context;
using QuanLyBenhVienNoiTru.Models.Entities;

namespace QuanLyBenhVienNoiTru.Controllers
{
    [Authorize(Roles = "Admin, Bác sĩ")]
    public class BacSiController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BacSi
        public ActionResult Index()
        {
            var bacSis = db.BacSis.Include(b => b.TaiKhoan);
            return View(bacSis.ToList());
        }

        // GET: BacSi/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bacSi = await db.BacSis
                .Include(b => b.TaiKhoan)
                .Include(b => b.Khoa)
                .Include(b => b.DieuTriBenhNhans)
                .FirstOrDefaultAsync(b => b.MaBacSi == id);
            if (bacSi == null)
            {
                return HttpNotFound();
            }
            return View(bacSi);
        }

        // GET: BacSi/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            // Lấy danh sách tài khoản có vai trò bác sĩ và chưa được gán cho bác sĩ nào
            var availableAccounts = db.TaiKhoans
                .Where(t => t.VaiTro == "Bác sĩ" && !db.BacSis.Any(b => b.MaTaiKhoan == t.MaTaiKhoan))
                .ToList();
            ViewBag.MaTaiKhoan = new SelectList(availableAccounts, "MaTaiKhoan", "TenDangNhap");
            return View();
        }

        // POST: BacSi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind("MaBacSi,HoTen,MaTaiKhoan,ChuyenKhoa,SoDienThoai")] QuanLyBenhVienNoiTru.Models.Entities.BacSi bacSi)
        {
            if (ModelState.IsValid)
            {
                db.BacSis.Add(bacSi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var availableAccounts = db.TaiKhoans
                .Where(t => t.VaiTro == "Bác sĩ" && !db.BacSis.Any(b => b.MaTaiKhoan == t.MaTaiKhoan))
                .ToList();
            ViewBag.MaTaiKhoan = new SelectList(availableAccounts, "MaTaiKhoan", "TenDangNhap", bacSi.MaTaiKhoan);
            return View(bacSi);
        }

        // GET: BacSi/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BacSi bacSi = db.BacSis.Find(id);
            if (bacSi == null)
            {
                return HttpNotFound();
            }
            
            var availableAccounts = db.TaiKhoans
                .Where(t => t.VaiTro == "Bác sĩ" && (t.MaTaiKhoan == bacSi.MaTaiKhoan || !db.BacSis.Any(b => b.MaTaiKhoan == t.MaTaiKhoan)))
                .ToList();
            ViewBag.MaTaiKhoan = new SelectList(availableAccounts, "MaTaiKhoan", "TenDangNhap", bacSi.MaTaiKhoan);
            return View(bacSi);
        }

        // POST: BacSi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind("MaBacSi", "HoTen", "MaTaiKhoan", "ChuyenKhoa", "SoDienThoai")] BacSi bacSi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bacSi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            var availableAccounts = db.TaiKhoans
                .Where(t => t.VaiTro == "Bác sĩ" && (t.MaTaiKhoan == bacSi.MaTaiKhoan || !db.BacSis.Any(b => b.MaTaiKhoan == t.MaTaiKhoan)))
                .ToList();
            ViewBag.MaTaiKhoan = new SelectList(availableAccounts, "MaTaiKhoan", "TenDangNhap", bacSi.MaTaiKhoan);
            return View(bacSi);
        }

        // GET: BacSi/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BacSi bacSi = db.BacSis.Include(b => b.TaiKhoan).FirstOrDefault(b => b.MaBacSi == id);
            if (bacSi == null)
            {
                return HttpNotFound();
            }
            return View(bacSi);
        }

        // POST: BacSi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            BacSi bacSi = db.BacSis.Find(id);

            // Kiểm tra xem bác sĩ có liên quan đến điều trị bệnh nhân không
            bool hasRelatedTreatments = db.DieuTriBenhNhans.Any(d => d.MaBacSi == id);
            if (hasRelatedTreatments)
            {
                ModelState.AddModelError("", "Không thể xóa bác sĩ này vì đã có thông tin điều trị bệnh nhân liên quan.");
                return View(bacSi);
            }

            db.BacSis.Remove(bacSi);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}