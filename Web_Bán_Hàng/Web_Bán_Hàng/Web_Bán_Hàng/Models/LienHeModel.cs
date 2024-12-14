using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Web_Bán_Hàng.Database.Validation;




namespace Web_Bán_Hàng.Models
{
	public class LienHeModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tiêu đề website")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập Bản đồ")]
		public string BanDo { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập phone")]
		public string SDT { get; set; }
        

        [Required(ErrorMessage = "Yêu cầu nhập thông tin liên hệ")]
		public string Mota { get; set; }

		public string Logo { get; set; }

       

        [NotMapped]
		[CheckFile]
		public IFormFile? ImageUpload { get; set; }

	}
}
