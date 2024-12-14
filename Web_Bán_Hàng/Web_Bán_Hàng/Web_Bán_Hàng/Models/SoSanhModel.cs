using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Bán_Hàng.Models
{
	public class SoSanhModel
	{
		[Key]
		public int Id { get; set; }

		public string ProductId { get; set; } // Khóa ngoại
		public string UserId { get; set; }

		[ForeignKey("ProductId")] // Chỉ định khóa ngoại
		public ProductModel Product { get; set; }
	}
}
