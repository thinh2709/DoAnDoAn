using QuanLyBenhVienNoiTru.Models.Entities;
using QuanLyBenhVienNoiTru.Models.ViewModels;

namespace QuanLyBenhVienNoiTru.Services
{
    public interface IBenhNhanService
    {
        Task<IEnumerable<BenhNhan>> GetAllBenhNhanAsync();
        Task<BenhNhan> GetBenhNhanByIdAsync(int id);
        Task<IEnumerable<BenhNhan>> GetBenhNhansByKhoaAsync(int maKhoa);
        Task AddBenhNhanAsync(BenhNhanViewModel benhNhanVM);
        Task UpdateBenhNhanAsync(BenhNhanViewModel benhNhanVM);
        Task DeleteBenhNhanAsync(int id);
    }
}