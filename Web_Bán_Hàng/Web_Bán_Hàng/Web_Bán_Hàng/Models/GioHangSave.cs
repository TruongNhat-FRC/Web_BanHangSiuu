using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models
{
	public class GioHangSave
	{
		[Key]
		public int Id { get; set; } // Khóa chính của bảng giỏ hàng

		[Required]
		public string UserId { get; set; } // ID người dùng (khóa ngoại từ bảng Users)

		[Required]
		public string ProductId { get; set; } // ID sản phẩm (khóa ngoại từ bảng Product)

		[Required]
		public string ProductName { get; set; } // Tên sản phẩm

		[Required]
		public int Quantity { get; set; } // Số lượng sản phẩm

		[Required]
		public decimal Price { get; set; } // Giá sản phẩm khi thêm vào giỏ hàng

		public string Image { get; set; } // Hình ảnh sản phẩm

		public DateTime CreatedAt { get; set; } = DateTime.Now; // Thời gian thêm vào giỏ hàng
	}
}
