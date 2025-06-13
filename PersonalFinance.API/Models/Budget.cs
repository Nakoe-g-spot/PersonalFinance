using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PersonalFinance.API.Models
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }

        // Liên kết với User (bắt buộc)
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

        // Liên kết với Category
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }

        // Mức ngân sách
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountLimit { get; set; }

        // xác định khoảng thời gian của ngân sách
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}