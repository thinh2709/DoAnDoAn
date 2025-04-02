using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBenhVienNoiTru.ViewModels
{
    public class ThongKeViewModel
    {
        // Thống kê tổng quát
        [Display(Name = "Tổng số bệnh nhân")]
        public int TongSoBenhNhan { get; set; }
        
        [Display(Name = "Số bệnh nhân đang điều trị")]
        public int SoBenhNhanDangDieuTri { get; set; }
        
        [Display(Name = "Số bệnh nhân đã xuất viện")]
        public int SoBenhNhanDaXuatVien { get; set; }
        
        [Display(Name = "Số bệnh nhân có BHYT")]
        public int SoBenhNhanCoBHYT { get; set; }
        
        [Display(Name = "Tổng doanh thu")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal TongDoanhThu { get; set; }
        
        [Display(Name = "Doanh thu chưa thanh toán")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal DoanhThuChuaThanhToan { get; set; }
        
        // Thống kê theo thời gian
        [Display(Name = "Thống kê từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }
        
        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? DenNgay { get; set; }
        
        // Thống kê số lượng bệnh nhân theo khoa
        public Dictionary<string, int> ThongKeBenhNhanTheoKhoa { get; set; } = new Dictionary<string, int>();
        
        // Thống kê doanh thu theo tháng
        public Dictionary<string, decimal> ThongKeDoanhThuTheoThang { get; set; } = new Dictionary<string, decimal>();
        
        // Thống kê số lượt điều trị theo loại
        public Dictionary<string, int> ThongKeSoLuotDieuTriTheoLoai { get; set; } = new Dictionary<string, int>();
        
        // Thống kê tỷ lệ bệnh nhân có BHYT theo khoa
        public Dictionary<string, double> ThongKeTyLeBHYTTheoKhoa { get; set; } = new Dictionary<string, double>();
        
        // Thống kê thời gian nằm viện trung bình theo khoa
        public Dictionary<string, double> ThongKeThoiGianTrungBinhTheoKhoa { get; set; } = new Dictionary<string, double>();
    }
    
    public class ThongKeBaoCaoViewModel
    {
        public IEnumerable<ThongKeBenhNhanViewModel> DanhSachBenhNhan { get; set; }
        public IEnumerable<ThongKeDoanhThuViewModel> DanhSachDoanhThu { get; set; }
        
        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }
        
        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? DenNgay { get; set; }
        
        [Display(Name = "Khoa")]
        public int? MaKhoa { get; set; }
    }
    
    public class ThongKeBenhNhanViewModel
    {
        public int MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string TenKhoa { get; set; }
        public DateTime NgayNhapVien { get; set; }
        public DateTime? NgayXuatVien { get; set; }
        public int SoNgayDieuTri { get; set; }
        public bool BaoHiemYTe { get; set; }
        public int SoLanDieuTri { get; set; }
        public decimal TongChiPhi { get; set; }
        public bool DaThanhToan { get; set; }
    }
    
    public class ThongKeDoanhThuViewModel
    {
        public string Thang { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoBenhNhan { get; set; }
        public decimal DoanhThuBHYT { get; set; }
        public decimal DoanhThuKhongBHYT { get; set; }
        public decimal TyLeTangGiam { get; set; }
    }
}