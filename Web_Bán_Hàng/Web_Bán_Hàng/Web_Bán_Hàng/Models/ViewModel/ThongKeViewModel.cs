using Web_Bán_Hàng.Models;

namespace Web_Ban_Hang.Models
{
    public class ThongKeViewModel
    {
        public decimal TongDoanhThu { get; set; }
        public int TongDonHang { get; set; }
        public int TongSanPhamBanRa { get; set; }
        public string SanPhamBanChayNhat { get; set; }
        public int TongSanPham { get; set; } 
        public int LuotDanhGia { get; set; } 

        public List<DuLieuBieuDoDoanhThu> DuLieuBieuDoDoanhThu { get; set; }
        public List<DuLieuBieuDoDonHang> DuLieuBieuDoDonHang { get; set; }
        public List<ProductModel> BanChay { get; set; }
    }

    public class DuLieuBieuDoDoanhThu
    {
        public string Ngay { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class DuLieuBieuDoDonHang
    {
        public string Ngay { get; set; }
        public int SoDonHang { get; set; }
    }
}
