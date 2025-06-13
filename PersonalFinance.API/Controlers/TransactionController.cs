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
    public class TransactionsController : ControllerBase
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<BankAccount> _bankAccountRepository;

        public TransactionsController(
            IRepository<Transaction> transactionRepository,
            IRepository<BankAccount> bankAccountRepository)
        {
            _transactionRepository = transactionRepository;
            _bankAccountRepository = bankAccountRepository;
        }

        // POST: api/Transactions
        // Tạo mới giao dịch trên một tài khoản ngân hàng thuộc về người dùng hiện tại
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
        {
            // Lấy thông tin user từ token để xác định chủ sở hữu
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim.Value);

            // Kiểm tra: BankAccount có tồn tại và thuộc về người dùng hiện tại
            var bankAccounts = await _bankAccountRepository.FindAsync(
            b => b.AccountId == transaction.BankAccountId && b.UserId == userId);
            var bankAccount = bankAccounts.FirstOrDefault();
            if (bankAccount == null)
                return BadRequest("Tài khoản ngân hàng không tồn tại hoặc không thuộc về bạn.");

            // Thực hiện thêm giao dịch vào repository
            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            return Ok(transaction);
        }

        // GET: api/Transactions
        // Lấy danh sách giao dịch của các tài khoản ngân hàng của người dùng hiện tại
        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);

            // Lấy những giao dịch mà tài khoản ngân hàng liên quan có UserId bằng userId hiện tại.
            var transactions = await _transactionRepository.FindAsync(t => t.BankAccount!.UserId == userId);

            return Ok(transactions);
        }
    }
}