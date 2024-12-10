using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; // Thuộc tính [Required]



namespace Web_Bán_Hàng.Models
{
	public class ChiTietDonHang
	{
        public int ID { get; set; }

        [Required]
        [ForeignKey("DonHang")]
        public string MaDonHang { get; set; }  // Khóa ngoại, liên kết với MaDonHang của bảng DonHangs

        public string MaNguoiDung { get; set; }
        public string MaSanPham { get; set; }
        public int Soluong { get; set; }
        public decimal ThanhTien { get; set; }
        public decimal Gia1mon { get; set; }

        // Liên kết với DonHangModel
        public virtual DonHangModel DonHang { get; set; }

        // Liên kết với ProductModel
        public virtual ProductModel Product { get; set; }
    }

    
    
        


    
}
