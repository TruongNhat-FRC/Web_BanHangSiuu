using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models
{
	public class UserModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage ="Vui lòng nhập thông tin ")]
		public string UserName { get; set; }
		[DataType(DataType.Password),Required(ErrorMessage ="Vui lòng nhập mật Khẩu")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin Email "),EmailAddress]
		public string Email { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập họ và tên của bạn ")]
        public string FullName {  get; set; }

		public string PhoneNumber { get; set; }

	}
}
