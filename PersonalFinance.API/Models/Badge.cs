using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinance.API.Models
{
	public class Badge {
		[Key]
		public int BadgeId { get; set; }           // ← EF mặc định coi đây là PK

		[ForeignKey("User")]
		public int UserId { get; set; }
		public required User User { get; set; }             // navigation

		[Required]
		public required string Title { get; set; }

		public DateTime AwardedDate { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	}
}