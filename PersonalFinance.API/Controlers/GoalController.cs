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
    public class GoalsController : ControllerBase
    {
        private readonly IRepository<Goal> _goalRepository;

        public GoalsController(IRepository<Goal> goalRepository)
        {
            _goalRepository = goalRepository;
        }

        //POST: api/Goals
        //Tạo mới 1 mục tiêu
        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] Goal goal)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            goal.UserId = userId;
            // Khởi tạo tiến độ ban đầu bằng 0
            goal.CurrentProgress = 0;

            // Kiểm tra hạn hoàn thành phải ở tương lai
            if (goal.Deadline <= DateTime.UtcNow)
                return BadRequest("Deadline phải là ngày trong tương lai.");

            await _goalRepository.AddAsync(goal);
            await _goalRepository.SaveChangesAsync();

            return Ok(goal);
        }


        // GET: api/Goals
        // Lấy danh sách mục tiêu của user hiện tại
        [HttpGet]
        public async Task<IActionResult> GetGoals()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            var goals = await _goalRepository.FindAsync(g => g.UserId == userId);
            return Ok(goals);
        }

        // PUT: api/Goals/{id}
        // Cập nhật thông tin mục tiêu dựa trên GoalId mà user sở hữu
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(int id, [FromBody] Goal updatedGoal)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            var goals = await _goalRepository.FindAsync(g => g.GoalId == id && g.UserId == userId);
            var goal = goals.FirstOrDefault();
            if (goal == null)
                return NotFound("Mục tiêu không tồn tại hoặc không thuộc về bạn.");

            // Cập nhật các trường cơ bản
            goal.Title = updatedGoal.Title;
            goal.Description = updatedGoal.Description;
            goal.TargetAmount = updatedGoal.TargetAmount;
            goal.Deadline = updatedGoal.Deadline;
            goal.IsCompleted = updatedGoal.IsCompleted;
            // Lưu ý: CurrentProgress có thể được cập nhật tự động hoặc qua endpoint riêng

            await _goalRepository.SaveChangesAsync();
            return Ok(goal);
        }

        // DELETE: api/Goals/{id}
        // Xoá mục tiêu của user hiện tại
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            var goals = await _goalRepository.FindAsync(g => g.GoalId == id && g.UserId == userId);
            var goal = goals.FirstOrDefault();
            if (goal == null)
                return NotFound("Mục tiêu không tồn tại hoặc không thuộc về bạn.");

            _goalRepository.Remove(goal);
            await _goalRepository.SaveChangesAsync();
            return Ok("Mục tiêu đã được xoá thành công.");
        }

    }
}
