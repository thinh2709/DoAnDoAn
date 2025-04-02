using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QuanLyBenhVienNoiTru.Models;

namespace QuanLyBenhVienNoiTru.ViewModels
{
    public class DieuTriViewModel
    {
        public int MaDieuTriBenhNhan { get; set; }

        [Display(Name = "Bệnh nhân")]
        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân")]
        public int MaBenhNhan { get; set; }

        [Display(Name = "Hình thức điều trị")]
        [Required(ErrorMessage = "Vui lòng chọn hình thức điều trị")]
        public int MaDieuTri { get; set; }

        [Display(Name = "Bác sĩ thực hiện")]
        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        public int MaBacSi { get; set; }

        [Display(Name = "Ngày thực hiện")]
        [Required(ErrorMessage = "Vui lòng chọn ngày thực hiện")]
        [DataType(DataType.Date)]
        public DateTime NgayThucHien { get; set; }

        [Display(Name = "Kết quả điều trị")]
        [StringLength(200)]
        public string KetQua { get; set; }

        // Thông tin bổ sung
        [Display(Name = "Tên bệnh nhân")]
        public string TenBenhNhan { get; set; }

        [Display(Name = "Tên hình thức điều trị")]
        public string TenDieuTri { get; set; }

        [Display(Name = "Chi phí")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal ChiPhi { get; set; }

        [Display(Name = "Tên bác sĩ")]
        public string TenBacSi { get; set; }

        [Display(Name = "Khoa")]
        public string TenKhoa { get; set; }

        // Danh sách lựa chọn để hiển thị trong dropdown
        public IEnumerable<BenhNhan> DanhSachBenhNhan { get; set; }
        public IEnumerable<HinhThucDieuTri> DanhSachHinhThucDieuTri { get; set; }
        public IEnumerable<BacSi> DanhSachBacSi { get; set; }
    }

    public class DieuTriListViewModel
    {
        public IEnumerable<DieuTriViewModel> DanhSachDieuTri { get; set; }
        public string TuKhoa { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? MaKhoa { get; set; }
        public int? MaBacSi { get; set; }
        public int? MaBenhNhan { get; set; }
        public IEnumerable<Khoa> DanhSachKhoa { get; set; }
        public IEnumerable<BacSi> DanhSachBacSi { get; set; }
        public int TongSoDieuTri { get; set; }
    }
}