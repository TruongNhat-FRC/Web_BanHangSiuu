using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models.ViewModel
{
	public class DangNhapViewModel
	{
		public int Id { get; set; }
	 	[Required(ErrorMessage ="Vui lòng nhập tên người dùng")]
		public string UserName { get; set; }
		[DataType(DataType.Password), Required(ErrorMessage ="Vui lòng nhập mật khẩu")]
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
