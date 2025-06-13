using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PersonalFinance.API.Models
{
    public class BankAccount
    {
        [Key]
        public int AccountId { get; set; }  // EF mặc định coi đây là PK

        [ForeignKey("User")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }  // Navigation property: không cần nhận dữ liệu từ client nên đánh dấu thành nullable và bỏ 'required'

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Currency { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}