﻿using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models
{
	public class DonHangModel
	{

        public int ID { get; set; }
        public string MaDonHang { get; set; }

        public string MaNguoiDung { get; set; }
        public DateTime NgayDat { get; set; }
        public int TrangThai { get; set; }
        public decimal TongTienCuoi { get; set; }
        // Liên kết với bảng ChiTietDonHang
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }
}
