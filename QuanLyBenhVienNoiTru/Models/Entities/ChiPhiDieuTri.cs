namespace QuanLyBenhVienNoiTru.Models.Entities
{
    public class ChiPhiDieuTri
    {
        public int MaChiPhi { get; set; }
        public int? MaBenhNhan { get; set; }
        public decimal TongChiPhi { get; set; }
        public bool DaThanhToan { get; set; }
        public DateTime? NgayLap { get; set; }

        // Navigation properties
        public virtual BenhNhan BenhNhan { get; set; }
    }
}