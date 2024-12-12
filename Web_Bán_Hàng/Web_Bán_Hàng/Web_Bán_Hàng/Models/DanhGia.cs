using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Web_Bán_Hàng.Models
{
	public class DanhGia
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string ProductId { get; set; }

		public string BinhLuan { get; set; }
		public string NameUser { get;set; }
		public string Email { get; set; }
		[Required]
		public DateTime CreatedAt { get; set; }

		public int Diem { get; set; }

		[ForeignKey("ProductId")]
		public ProductModel Product { get; set; }

		
	}
}
