using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_Bán_Hàng.Database.Validation;
namespace Web_Bán_Hàng.Models
{
	public class ProductModel
	{
		[Key]
		public string Id { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên sản phầm")]

		public string Name { get; set; }
        public string Slug { get; set; } = string.Empty; // Đảm bảo có giá trị mặc định

        [Required]
        [MinLength(10, ErrorMessage = "Mô tả sản phẩm phải có ít nhất 10 ký tự.")]
        public string Description { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Lượt mua phải là một số không âm.")]
		public int PurchaseCount { get; set; } = 0;



		[Required]
        [Range(0, double.MaxValue, ErrorMessage = "Yêu cầu giá sản phẩm phải là một số dương.")]
        public decimal Price { get; set; }
		public int BrandId { get; set; }
		public int CategoryId { get; set; }
		public CategoryModel Category { get; set; }
		public BrandModel Brand { get; set; }
		public string Image { get; set; } = "notname.png";
		[NotMapped]
		[CheckFile]
		public IFormFile? ImageUpLoad { get; set; }
        [Range(0, 100, ErrorMessage = "Số lượng phải nằm trong khoảng từ 0 đến 100.")]
        public int Quantity { get; set; } = 0; 
        [Required]
        public bool IsVisible { get; set; } = true;
		public List<DanhGia> DanhGias { get; set; } // Sửa để chứa danh sách đánh giá

	}
}
