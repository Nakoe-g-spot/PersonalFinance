using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PersonalFinance.API.Models
{
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }

        // Liên kết với người dùng tạo mục tiêu
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        // Tên hoặc tiêu đề của mục tiêu
        [Required]
        public string Title { get; set; } = string.Empty;

        // Mô tả chi tiết về mục tiêu
        public string? Description { get; set; }

        // Số tiền mục tiêu cần đạt được
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetAmount { get; set; }

        // Mức tiến độ đã đạt được; ban đầu là 0, có thể cập nhật tự động (tích hợp giao dịch) hoặc qua API
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentProgress { get; set; } = 0;

        // Deadline (ngày hoàn thành mục tiêu)
        [Required]
        public DateTime Deadline { get; set; }

        // Ghi lại thời điểm tạo mục tiêu
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Trạng thái hoàn thành mục tiêu (có thể được tự động cập nhật khi CurrentProgress đạt TargetAmount)
        public bool IsCompleted { get; set; } = false;
    }
}