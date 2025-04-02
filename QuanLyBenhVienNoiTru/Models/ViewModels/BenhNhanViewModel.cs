using System.ComponentModel.DataAnnotations;

namespace QuanLyBenhVienNoiTru.Models.ViewModels
{
    public class BenhNhanViewModel
    {
        public int MaBenhNhan { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }

        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Bảo hiểm y tế")]
        public bool BaoHiemYTe { get; set; }

        [Required(ErrorMessage = "Ngày nhập viện không được để trống")]
        [Display(Name = "Ngày nhập viện")]
        [DataType(DataType.Date)]
        public DateTime NgayNhapVien { get; set; }

        [Display(Name = "Ngày xuất viện")]
        [DataType(DataType.Date)]
        public DateTime? NgayXuatVien { get; set; }

        [Required(ErrorMessage = "Khoa không được để trống")]
        [Display(Name = "Khoa")]
        public int MaKhoa { get; set; }

        public string TenKhoa { get; set; }
    }
}