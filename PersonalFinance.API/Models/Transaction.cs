using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PersonalFinance.API.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [ForeignKey("BankAccount")]
        public int BankAccountId { get; set; }

        // Navigation property: đánh dấu nullable và bỏ 'required' vì client không cung cấp dữ liệu này.
        [JsonIgnore]
        public BankAccount? BankAccount { get; set; }
        // Thêm thuộc tính CategoryId, làm khóa ngoại đến bảng Category.
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        // Navigation property cho Category.
        [JsonIgnore]
        public Category? Category { get; set; }

        // Thêm thuộc tính Type để chỉ loại giao dịch (ví dụ: "expense" hoặc "income").
        [Required]
        public string Type { get; set; } = string.Empty;


        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}