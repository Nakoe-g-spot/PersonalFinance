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
    public class BadgesController : ControllerBase
    {
        private readonly IRepository<Badge> _badgeRepository;
        public BadgesController(IRepository<Badge> badgeRepository)
        {
            _badgeRepository = badgeRepository;
        }

        // GET: api/Badges
        // Lấy danh sách badge của người dùng hiện tại
        [HttpGet]
        public async Task<IActionResult> GetBadges()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            var badges = await _badgeRepository.FindAsync(b => b.UserId == userId);
            return Ok(badges);
        }

        // PUT: api/Badges/{id}
        // Cập nhật thông tin badge của người dùng hiện tại
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBadge(int id, [FromBody] Badge updatedBadge)
        {
            // Lấy UserId từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);

            // Tìm badge của user có BadgeId tương ứng
            var badges = await _badgeRepository.FindAsync(b => b.BadgeId == id && b.UserId == userId);
            var badge = badges.FirstOrDefault();
            if (badge == null)
                return NotFound("Badge không tồn tại hoặc không thuộc về bạn.");

            // Cập nhật các trường được phép thay đổi
            badge.Name = updatedBadge.Name;
            badge.Description = updatedBadge.Description;
            badge.Icon = updatedBadge.Icon;
            // badge.AwardedAt = updatedBadge.AwardedAt;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _badgeRepository.SaveChangesAsync();

            return Ok(badge);
        }


        // POST: api/Badges
        // Tạo mới badge và gán cho người dùng hiện tại
        [HttpPost]
        public async Task<IActionResult> CreateBadge([FromBody] Badge badge)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            badge.UserId = userId;
            badge.AwardedAt = DateTime.UtcNow;
            await _badgeRepository.AddAsync(badge);
            await _badgeRepository.SaveChangesAsync();
            return Ok(badge);
        }

        // DELETE: api/Badges/{id}
        // Xoá badge của người dùng hiện tại (nếu cần)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBadge(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);
            var badges = await _badgeRepository.FindAsync(b => b.BadgeId == id && b.UserId == userId);
            var badge = badges.FirstOrDefault();
            if (badge == null)
                return NotFound("Badge không tồn tại hoặc không thuộc về bạn.");

            _badgeRepository.Remove(badge);
            await _badgeRepository.SaveChangesAsync();
            return Ok("Badge đã được xoá thành công.");
        }
    }
}