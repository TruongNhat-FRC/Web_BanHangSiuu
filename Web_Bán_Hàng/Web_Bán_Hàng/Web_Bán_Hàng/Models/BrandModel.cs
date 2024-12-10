using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models
{
	public class BrandModel
	{
		[Key]
		public int Id { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên thương hiệu")]

		public string Name { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả doanh mục")]

		public string Description { get; set; }
		public string Slug { get; set; }
		public int status { get; set; }
	}
}
