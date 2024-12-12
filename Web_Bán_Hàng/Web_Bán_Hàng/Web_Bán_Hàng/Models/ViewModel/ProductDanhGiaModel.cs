using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models.ViewModel
{
	public class ProductDanhGiaModel
	{
		public ProductModel ProductDetail { get; set; }
		public List<DanhGia> DanhGias { get; set; } // Danh sách đánh giá

		[Required(ErrorMessage = "Tên người dùng không được để trống")]
		[StringLength(50, ErrorMessage = "Tên không được dài quá 50 ký tự")]
		public string NameUser { get; set; }

		[Required(ErrorMessage = "Email không được để trống")]
		[EmailAddress(ErrorMessage = "Email không hợp lệ")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Bình luận không được để trống")]
		public string BinhLuan { get; set; }

		public int Diem { get; set; }
	}
}
