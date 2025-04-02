using QuanLyBenhVienNoiTru.Models.Entities;
using QuanLyBenhVienNoiTru.Models.ViewModels;
using QuanLyBenhVienNoiTru.Repositories;

namespace QuanLyBenhVienNoiTru.Services
{
    public class BenhNhanService : IBenhNhanService
    {
        private readonly IBenhNhanRepository _benhNhanRepository;

        public BenhNhanService(IBenhNhanRepository benhNhanRepository)
        {
            _benhNhanRepository = benhNhanRepository;
        }

        public async Task<IEnumerable<BenhNhan>> GetAllBenhNhanAsync()
        {
            return await _benhNhanRepository.GetBenhNhansWithKhoaAsync();
        }

        public async Task<BenhNhan> GetBenhNhanByIdAsync(int id)
        {
            return await _benhNhanRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BenhNhan>> GetBenhNhansByKhoaAsync(int maKhoa)
        {
            return await _benhNhanRepository.GetBenhNhansByKhoaAsync(maKhoa);
        }

        public async Task AddBenhNhanAsync(BenhNhanViewModel benhNhanVM)
        {
            var benhNhan = new BenhNhan
            {
                HoTen = benhNhanVM.HoTen,
                NgaySinh = benhNhanVM.NgaySinh,
                GioiTinh = benhNhanVM.GioiTinh,
                SoDienThoai = benhNhanVM.SoDienThoai,
                DiaChi = benhNhanVM.DiaChi,
                BaoHiemYTe = benhNhanVM.BaoHiemYTe,
                NgayNhapVien = benhNhanVM.NgayNhapVien,
                NgayXuatVien = benhNhanVM.NgayXuatVien,
                MaKhoa = benhNhanVM.MaKhoa
            };

            await _benhNhanRepository.AddAsync(benhNhan);
        }

        public async Task UpdateBenhNhanAsync(BenhNhanViewModel benhNhanVM)
        {
            var benhNhan = await _benhNhanRepository.GetByIdAsync(benhNhanVM.MaBenhNhan);
            if (benhNhan != null)
            {
                benhNhan.HoTen = benhNhanVM.HoTen;
                benhNhan.NgaySinh = benhNhanVM.NgaySinh;
                benhNhan.GioiTinh = benhNhanVM.GioiTinh;
                benhNhan.SoDienThoai = benhNhanVM.SoDienThoai;
                benhNhan.DiaChi = benhNhanVM.DiaChi;
                benhNhan.BaoHiemYTe = benhNhanVM.BaoHiemYTe;
                benhNhan.NgayNhapVien = benhNhanVM.NgayNhapVien;
                benhNhan.NgayXuatVien = benhNhanVM.NgayXuatVien;
                benhNhan.MaKhoa = benhNhanVM.MaKhoa;

                await _benhNhanRepository.UpdateAsync(benhNhan);
            }
        }

        public async Task DeleteBenhNhanAsync(int id)
        {
            await _benhNhanRepository.DeleteAsync(id);
        }
    }
}