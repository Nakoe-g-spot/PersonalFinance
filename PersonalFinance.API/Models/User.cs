using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PersonalFinance.API.Models
{
	public class User {
		[Key]
		public int UserId { get; set; }           // ← EF mặc định coi đây là PK

        [Required]
		public required string Name { get; set; }

		public required string Email { get; set; }

		public required string PasswordHash { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public ICollection<Category> Categories { get; set; } = new List<Category>();
	}
}