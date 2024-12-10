using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models
{
	public class CategoryModel
	{
		[Key]
		public int Id { get; set; }
		[Required, MinLength(4,ErrorMessage ="Yêu cầu nhập tên doanh mục")]
		public string Name { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả doanh mục")]
		public string Description { get; set; }
		
		/*Slug dùng để chỉ định dạng phần cuối trong các chuỗi tìm kiếm url*/
		public string Slug { get; set; } = string.Empty;
        public int Status { get; set; }


	}
}
