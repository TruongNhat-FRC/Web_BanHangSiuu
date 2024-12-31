using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models
{
	public class GioHangSave
	{
		[Key]
		public int Id { get; set; } 

		[Required]
		public string UserId { get; set; } 

		[Required]
		public string ProductId { get; set; } 

		[Required]
		public string ProductName { get; set; }

		[Required]
		public int Quantity { get; set; } 

		[Required]
		public decimal Price { get; set; } 

		public string Image { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now; 
	}
}
