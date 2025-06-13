using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Models;
using PersonalFinance.API.Repositories;
using System.Security.Claims;

namespace PersonalFinance.API.Controllers
{
    // Điều chỉnh route và bật xác thực cho toàn bộ controller:
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IRepository<BankAccount> _bankAccountRepository;

        public BankAccountsController(IRepository<BankAccount> bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        // Endpoint POST: api/bankaccounts để thêm ngân hàng
        [HttpPost]
        public async Task<IActionResult> AddBankAccount([FromBody] BankAccount bankAccount)
        {
            // Lấy userId từ token; thông tin này được chứa trong claim NameIdentifier khi đăng nhập thành công.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            // Ép kiểu userId (chúng ta giả định userId là số nguyên)
            bankAccount.UserId = int.Parse(userIdClaim.Value);

            // Lưu ngân hàng vào cơ sở dữ liệu qua repository
            await _bankAccountRepository.AddAsync(bankAccount);
            await _bankAccountRepository.SaveChangesAsync();

            return Ok(bankAccount);
        }
        [HttpGet]
        public async Task<IActionResult> GetBankAccounts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim.Value);
            var bankAccounts = await _bankAccountRepository.FindAsync(b => b.UserId == userId);
            return Ok(bankAccounts);
        }

    }
}