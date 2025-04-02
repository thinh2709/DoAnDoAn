using Microsoft.EntityFrameworkCore;
using QuanLyBenhVienNoiTru.Models.Context;
using QuanLyBenhVienNoiTru.Models.Entities;

namespace QuanLyBenhVienNoiTru.Repositories
{
    public class BenhNhanRepository : GenericRepository<BenhNhan>, IBenhNhanRepository
    {
        public BenhNhanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BenhNhan>> GetBenhNhansByKhoaAsync(int maKhoa)
        {
            return await _dbSet.Where(b => b.MaKhoa == maKhoa).ToListAsync();
        }

        public async Task<IEnumerable<BenhNhan>> GetBenhNhansWithKhoaAsync()
        {
            return await _dbSet.Include(b => b.Khoa).ToListAsync();
        }
    }
}