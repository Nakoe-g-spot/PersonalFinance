using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.API.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;  // Sử dụng tên Password thay vì PasswordHash vì này là giá trị nhập vào từ người dùng.
    }
}