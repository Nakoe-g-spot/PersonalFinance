using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Models;
using PersonalFinance.API.Repositories;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


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


        // GET: api/Transactions
        // Lọc và sắp xếp giao dịch theo nhiều tiêu chí
        [HttpGet("summary")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount,
            [FromQuery] string? transactionType,
            [FromQuery] string? sortBy
        )
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");
            int userId = int.Parse(userIdClaim);

            // Bắt đầu truy vấn của người dùng
            var query = _transactionRepository.Query.Where(t => t.BankAccount!.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);
            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);
            if (minAmount.HasValue)
                query = query.Where(t => t.Amount >= minAmount.Value);
            if (maxAmount.HasValue)
                query = query.Where(t => t.Amount <= maxAmount.Value);
            if (!string.IsNullOrEmpty(transactionType))
            {
                //Type của Transaction có kiểu string đại diện cho loại giao dịch
                query = query.Where(t => t.Type.ToLower() == transactionType.ToLower());
            }

            //tham số truyền vào
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "date":
                        query = query.OrderBy(t => t.TransactionDate);
                        break;
                    case "amount":
                        query = query.OrderBy(t => t.Amount);
                        break;
                    default:
                        query = query.OrderBy(t => t.TransactionDate);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(t => t.TransactionDate);
            }

            var transactions = await query.ToListAsync();
            return Ok(transactions);
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
        [HttpGet("list")]
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