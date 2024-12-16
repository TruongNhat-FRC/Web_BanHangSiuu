using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models
{
	public class KhuyenMaiModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên mã giảm giá")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập mô tả")]

		public string Mota { get; set; }
		public DateTime NgayBatDau {  get; set; }
		public DateTime NgayKetThuc {  get; set; }
		[Required(ErrorMessage = "Vui lòng nhập số lượng mã giảm giá")]
		public int SoLuong {  get; set; }
		public int trangthai {  get; set; }
		[Required(ErrorMessage = "Vui lòng nhập  giá")]
		public decimal Gia {  get; set; }
	}
}
