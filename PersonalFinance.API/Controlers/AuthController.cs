using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Models;
using PersonalFinance.API.DTOs;
using PersonalFinance.API.Repositories;
using PersonalFinance.API.Services.Interfaces;       // Chứa giao diện IUserService.

namespace PersonalFinance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Dependency Injection: nhận đối tượng IUserService từ DI container.
        private readonly IUserService _userService;
        public AuthController(IUserService userService) => _userService = userService;

        //get listuser
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers([FromServices] IRepository<User> userRepo)
        {
            var users = await userRepo.GetAllAsync();
            return Ok(users);
        }
        // Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Gọi service để đăng ký.
            var created = await _userService.RegisterAsync(request.Name, request.Email, request.Password!);
            if (created == null)
                return BadRequest("Email đã tồn tại");

            return Ok(created);
        }


        /// Endpoint đăng nhập.
        /// Body yêu cầu: { "email": "email", "passwordHash": "mật khẩu" }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Gọi service để xác thực người dùng.
            var token = await _userService.AuthenticateAsync(request.Email, request.Password!);
            if (token == null)
                return Unauthorized("Email hoặc mật khẩu không đúng");

            // Trả về token dưới dạng JSON.
            return Ok(new { Token = token });
        }
    }
}