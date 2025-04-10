namespace QuanLyBenhVienNoiTru.Models.Entities
{
public class TaiKhoan
{
    public int MaTaiKhoan { get; set; }
    public string TenDangNhap { get; set; }
    public string MatKhau { get; set; }
    public string VaiTro { get; set; }

    // Navigation properties
    public virtual BacSi BacSi { get; set; }
    public virtual KhachThamBenh KhachThamBenh { get; set; }
}
}