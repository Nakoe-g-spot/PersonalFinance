using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.API.DTOs
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class VerifyTwoFactorDto
    {
        public string Code { get; set; } = string.Empty;
    }

}