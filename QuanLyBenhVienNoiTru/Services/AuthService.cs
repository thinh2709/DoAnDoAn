using Microsoft.EntityFrameworkCore;
using QuanLyBenhVienNoiTru.Models.Context;
using QuanLyBenhVienNoiTru.Models.ViewModels;

namespace QuanLyBenhVienNoiTru.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AuthenticateAsync(LoginViewModel loginVM)
        {
            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(u => u.TenDangNhap == loginVM.TenDangNhap && u.MatKhau == loginVM.MatKhau);

            return user != null;
        }

        public async Task<string> GetUserRoleAsync(string username)
        {
            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(u => u.TenDangNhap == username);

            return user?.VaiTro;
        }
    }
}