using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PersonalFinance.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        // Khóa ngoại liên kết với User
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation property (không cần client truyền dữ liệu này)
        [JsonIgnore]
        public User? User { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        // Nếu bạn có trường Icon, cân nhắc thêm nếu cần
        //public string? Icon { get; set; }
    }
}