using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PersonalFinance.API.Models;
using PersonalFinance.API.Repositories;
using PersonalFinance.API.Services.Interfaces;

namespace PersonalFinance.API.Services
{
	public class UserService : IUserService
	{
		private readonly IRepository<User> _userRepo;// Repository cho User
		private readonly IConfiguration _cfg; // Cấu hình từ appsettings.json

		public UserService(IRepository<User> userRepo, IConfiguration cfg)
		{
			_userRepo = userRepo;
			_cfg = cfg;
		}

		public async Task<User?> RegisterAsync(string name, string email, string password)
		{
			// Kiểm tra email đã tồn tại hay chưa
			var exists = await _userRepo.FindAsync(u => u.Email == email);
			if (exists.Any()) return null; // Nếu đã tồn tại, trả về null

			// Tạo instance cho user mới. Sử dụng BCrypt để mã hoá mật khẩu
			var user = new User
			{
				Name = name,
				Email = email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
			};
			await _userRepo.AddAsync(user);
			await _userRepo.SaveChangesAsync();
			return user;
		}

		public async Task<string?> AuthenticateAsync(string email, string password)
		{
			// Tìm user theo email
			var user = (await _userRepo.FindAsync(u => u.Email == email))
							.FirstOrDefault();
			if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
				return null; // Nếu không khớp, trả về null


			// Tạo JWT token chứa thông tin claims của user
			var jwtKey = _cfg["Jwt:Key"];
			if (jwtKey is null)
			{
				throw new Exception("Missing Jwt:Key setting in configuration!");
			}
			var key = Encoding.UTF8.GetBytes(jwtKey);
			var token = new JwtSecurityToken(
				issuer: _cfg["Jwt:Issuer"],
				audience: _cfg["Jwt:Audience"],
				claims: new[] {
					new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
					new Claim(ClaimTypes.Email, user.Email)
				},
				expires: DateTime.UtcNow.AddHours(2), // Token hết hạn sau 2 giờ
				signingCredentials: new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256)); 

			// Trả về token dạng chuỗi
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}