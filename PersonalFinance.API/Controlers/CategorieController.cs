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
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoriesController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // POST: api/Categories
        // Tạo mới danh mục giao dịch cho người dùng đang đăng nhập
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            // Lấy thông tin user từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            // Gán cho danh mục thuộc về người dùng
            category.UserId = int.Parse(userIdClaim.Value);

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return Ok(category);
        }

        // GET: api/Categories
        // Lấy danh sách danh mục giao dịch của người dùng hiện tại
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim.Value);
            var categories = await _categoryRepository.FindAsync(c => c.UserId == userId);

            return Ok(categories);
        }

        // PUT: api/Categories/{id}
        // Sửa thông tin của một danh mục mà người dùng đang sở hữu
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category updatedCategory)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);

            // Lấy danh mục cần sửa thuộc về user hiện tại
            var categories = await _categoryRepository.FindAsync(c => c.CategoryId == id && c.UserId == userId);
            var category = categories.FirstOrDefault();
            if (category == null)
                return NotFound("Danh mục không tồn tại.");

            // Cập nhật thông tin từ dữ liệu từ client
            category.Name = updatedCategory.Name;
            category.Description = updatedCategory.Description;
            // Nếu có các thuộc tính khác cần sửa, bổ sung thêm ở đây

            await _categoryRepository.SaveChangesAsync();
            return Ok(category);
        }


        // DELETE: api/Categories/{id} - Xoá danh mục
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Lấy thông tin user từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không tìm thấy thông tin người dùng từ token.");

            int userId = int.Parse(userIdClaim);

            // Tìm danh mục của user theo id được cung cấp
            var categories = await _categoryRepository.FindAsync(c => c.CategoryId == id && c.UserId == userId);
            var category = categories.FirstOrDefault();
            if (category == null)
                return NotFound("Danh mục không tồn tại.");

            // Xoá danh mục và lưu thay đổi
            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync();

            return Ok("Danh mục đã được xoá thành công.");
        }

    }
}