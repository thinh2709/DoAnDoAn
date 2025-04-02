using QuanLyBenhVienNoiTru.Models.Entities;

namespace QuanLyBenhVienNoiTru.Repositories
{
    public interface IBenhNhanRepository : IGenericRepository<BenhNhan>
    {
        Task<IEnumerable<BenhNhan>> GetBenhNhansByKhoaAsync(int maKhoa);
        Task<IEnumerable<BenhNhan>> GetBenhNhansWithKhoaAsync();
    }
}