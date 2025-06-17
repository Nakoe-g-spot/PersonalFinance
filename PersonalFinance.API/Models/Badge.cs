using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PersonalFinance.API.Models
{
    public class Badge
    {
        [Key]
        public int BadgeId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Có thể dùng để lưu tên file, URL, hoặc mã icon nào đó
        public string? Icon { get; set; }

        public DateTime AwardedAt { get; set; } = DateTime.UtcNow;

        // Liên kết với người dùng - mỗi badge thuộc về một user
        [ForeignKey("User")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
    }
}