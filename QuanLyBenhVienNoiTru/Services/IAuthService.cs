using QuanLyBenhVienNoiTru.Models.ViewModels;

namespace QuanLyBenhVienNoiTru.Services
{
    public interface IAuthService
    {
        Task<bool> AuthenticateAsync(LoginViewModel loginVM);
        Task<string> GetUserRoleAsync(string username);
    }
}
