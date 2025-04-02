using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QuanLyBenhVienNoiTru.Models;

namespace QuanLyBenhVienNoiTru.ViewModels
{
    public class LichThamBenhViewModel
    {
        public int MaLich { get; set; }
        
        [Display(Name = "Khách thăm")]
        [Required(ErrorMessage = "Vui lòng chọn khách thăm")]
        public int MaKhach { get; set; }
        
        [Display(Name = "Bệnh nhân")]
        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân")]
        public int MaBenhNhan { get; set; }
        
        [Display(Name = "Thời gian thăm")]
        [Required(ErrorMessage = "Vui lòng chọn thời gian thăm")]
        [DataType(DataType.DateTime)]
        public DateTime ThoiGianTham { get; set; }
        
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }
        
        // Thông tin bổ sung
        [Display(Name = "Tên khách thăm")]
        public string TenKhach { get; set; }
        
        [Display(Name = "Số điện thoại")]
        public string SoDienThoaiKhach { get; set; }
        
        [Display(Name = "Mối quan hệ")]
        public string MoiQuanHe { get; set; }
        
        [Display(Name = "Tên bệnh nhân")]
        public string TenBenhNhan { get; set; }
        
        [Display(Name = "Khoa")]
        public string TenKhoa { get; set; }
        
        [Display(Name = "Phòng")]
        public string Phong { get; set; }
        
        // Danh sách lựa chọn để hiển thị trong dropdown
        public IEnumerable<QuanLyBenhVienNoiTru.Models.Entities.KhachThamBenh> DanhSachKhachTham { get; set; }
        public IEnumerable<QuanLyBenhVienNoiTru.Models.Entities.BenhNhan> DanhSachBenhNhan { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(200)]
        public string GhiChu { get; set; }
    }
    
    public class LichThamBenhListViewModel
    {
        public IEnumerable<LichThamBenhViewModel> DanhSachLichTham { get; set; }
        public string TuKhoa { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TrangThai { get; set; }
        public int? MaBenhNhan { get; set; }
        public List<string> DanhSachTrangThai { get; set; } = new List<string> { "Chờ duyệt", "Đã duyệt", "Hủy" };
        public int TongSoLichTham { get; set; }
    }
}