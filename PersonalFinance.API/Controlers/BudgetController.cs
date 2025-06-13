using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Models;
using PersonalFinance.API.Repositories;
using System.Security.Claims;
using System.Linq;

namespace PersonalFinance.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetsController : ControllerBase
    {
        private readonly IRepository<Budget> _budgetRepository;

        public BudgetsController(IRepository<Budget> budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        // POST: api/Budgets
        // Tạo mới ngân sách cho user hiện tại
        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] Budget budget)
        {
            // Lấy thông tin user từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            budget.UserId = userId;

            // Kiểm tra logic: Có thể validate rằng StartDate và EndDate hợp lệ, AmountLimit > 0, v.v.
            if (budget.StartDate >= budget.EndDate)
                return BadRequest("StartDate phải nhỏ hơn EndDate.");

            await _budgetRepository.AddAsync(budget);
            await _budgetRepository.SaveChangesAsync();

            return Ok(budget);
        }

        // GET: api/Budgets
        // Lấy danh sách ngân sách của user hiện tại
        [HttpGet]
        public async Task<IActionResult> GetBudgets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            var budgets = await _budgetRepository.FindAsync(b => b.UserId == userId);
            return Ok(budgets);
        }

        // PUT: api/Budgets/{id}
        // Cập nhật ngân sách của user hiện tại
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] Budget updatedBudget)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            // Lấy ngân sách cần cập nhật
            var budgets = await _budgetRepository.FindAsync(b => b.BudgetId == id && b.UserId == userId);
            var budget = budgets.FirstOrDefault();
            if (budget == null)
                return NotFound("Ngân sách không tồn tại hoặc không thuộc về bạn.");

            // Cập nhật các trường thông tin cần thiết
            budget.AmountLimit = updatedBudget.AmountLimit;
            budget.StartDate = updatedBudget.StartDate;
            budget.EndDate = updatedBudget.EndDate;
            budget.CategoryId = updatedBudget.CategoryId;
            // Không cập nhật CreatedAt vì đây là thời điểm khởi tạo

            await _budgetRepository.SaveChangesAsync();
            return Ok(budget);
        }

        // DELETE: api/Budgets/{id}
        // Xoá ngân sách của user hiện tại
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            var budgets = await _budgetRepository.FindAsync(b => b.BudgetId == id && b.UserId == userId);
            var budget = budgets.FirstOrDefault();
            if (budget == null)
                return NotFound("Ngân sách không tồn tại hoặc không thuộc về bạn.");

            _budgetRepository.Remove(budget);
            await _budgetRepository.SaveChangesAsync();
            return Ok("Ngân sách đã được xoá thành công.");
        }
    }
}