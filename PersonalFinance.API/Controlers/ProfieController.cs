using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Models;
using PersonalFinance.API.Repositories;
using System.Security.Claims;

namespace PersonalFinance.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class ProfileController : ControllerBase
	{
		private readonly IRepository<User> _userRepository;
		private readonly IPasswordHasher _passwordHasher; // Service băm mật khẩu
		public ProfileController(IRepository<User> userRepository, IPasswordHasher passwordHasher)
		{
			_userRepository = userRepository;
			_passwordHasher = passwordHasher;
		}

		// PUT: api/Profile/changePassword
		// Đổi mật khẩu của người dùng hiện tại
		[HttpPut("changePassword")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdClaim))
				return Unauthorized("Không tìm thấy thông tin người dùng từ token.");
			int userId = int.Parse(userIdClaim);
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				return NotFound("User not found.");

			// Kiểm tra mật khẩu hiện tại
			if (!_passwordHasher.VerifyPassword(user.PasswordHash, dto.CurrentPassword))
				return BadRequest("Mật khẩu hiện tại không đúng.");

			if (dto.NewPassword != dto.ConfirmPassword)
				return BadRequest("Mật khẩu mới và xác nhận phải trùng nhau.");

			// Cập nhật mật khẩu mới
			user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
			await _userRepository.SaveChangesAsync();
			return Ok("Đổi mật khẩu thành công.");
		}

		// POST: api/Profile/setup2fa
		// Cài đặt 2FA, trả về secret key và QR code URL cho Authenticator app
		[HttpPost("setup2fa")]
		public async Task<IActionResult> SetupTwoFactorAuth()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdClaim))
				return Unauthorized("Không tìm thấy thông tin người dùng từ token.");
			int userId = int.Parse(userIdClaim);
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				return NotFound("User not found.");

			// Sinh secret key cho 2FA (ví dụ sử dụng thuật toán TOTP)
			var secret = TwoFactorAuthService.GenerateSecretKey();
			user.TwoFactorSecret = secret;
			user.IsTwoFactorEnabled = false; // chưa bật cho đến khi xác thực
			await _userRepository.SaveChangesAsync();

			// Tạo URL QR theo định dạng otpauth://, hỗ trợ việc quét vào Authenticator app (Google Authenticator, Microsoft Authenticator,...)
			var qrCodeUrl = TwoFactorAuthService.GenerateQrCodeUrl(user.Username, secret);
			return Ok(new { secret, qrCodeUrl });
		}

		// POST: api/Profile/verify2fa
		// Xác thực 2FA bằng cách kiểm tra mã OTP cung cấp
		[HttpPost("verify2fa")]
		public async Task<IActionResult> VerifyTwoFactorAuth([FromBody] VerifyTwoFactorDto dto)
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdClaim))
				return Unauthorized("Không tìm thấy thông tin người dùng từ token.");
			int userId = int.Parse(userIdClaim);
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				return NotFound("User not found.");

			// Kiểm tra mã xác thực dựa trên secret đã lưu
			if (!TwoFactorAuthService.ValidateCode(user.TwoFactorSecret, dto.Code))
				return BadRequest("Mã xác thực không hợp lệ.");

			user.IsTwoFactorEnabled = true;
			await _userRepository.SaveChangesAsync();
			return Ok("Xác thực 2 bước đã được bật thành công.");
		}

		// POST: api/Profile/disable2fa
		// Tắt xác thực 2 bước cho người dùng hiện tại
		[HttpPost("disable2fa")]
		public async Task<IActionResult> DisableTwoFactorAuth()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdClaim))
				return Unauthorized("Không tìm thấy thông tin người dùng từ token.");
			int userId = int.Parse(userIdClaim);
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				return NotFound("User not found.");

			user.IsTwoFactorEnabled = false;
			user.TwoFactorSecret = null;
			await _userRepository.SaveChangesAsync();
			return Ok("Xác thực 2 bước đã được tắt thành công.");
		}
	}
}