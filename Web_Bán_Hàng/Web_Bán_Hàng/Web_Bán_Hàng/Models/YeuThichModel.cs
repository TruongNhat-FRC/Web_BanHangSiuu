using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models
{
	public class YeuThichModel
	{

		[Key]
		public int Id { get; set; }

		public string ProductId { get; set; } // Khóa ngoại
		public string UserId { get; set; }

		[ForeignKey("ProductId")] // Chỉ định khóa ngoại
		public ProductModel Product { get; set; }
	}
}
