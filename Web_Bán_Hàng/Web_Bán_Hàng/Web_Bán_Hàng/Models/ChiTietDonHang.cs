using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; // Thuộc tính [Required]



namespace Web_Bán_Hàng.Models
{
	public class ChiTietDonHang
	{
        public int ID { get; set; }

        [Required]
        [ForeignKey("DonHang")]
        public string MaDonHang { get; set; }  

        public string MaNguoiDung { get; set; }
        public string MaSanPham { get; set; }
        public int Soluong { get; set; }
        public decimal ThanhTien { get; set; }
        public decimal Gia1mon { get; set; }

        public virtual DonHangModel DonHang { get; set; }

        public virtual ProductModel Product { get; set; }
    }

    
    
        


    
}
