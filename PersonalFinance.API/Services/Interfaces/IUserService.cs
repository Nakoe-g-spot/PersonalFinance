using PersonalFinance.API.Models;
namespace PersonalFinance.API.Services.Interfaces
{
    public interface IUserService
    {
        // Đăng ký (Register) một người dùng mới
        Task<User?> RegisterAsync(string name, string email, string password);

        // Xác thực người dùng, trả về JWT token nếu thành công
        Task<string?> AuthenticateAsync(string email, string password);
    }
}