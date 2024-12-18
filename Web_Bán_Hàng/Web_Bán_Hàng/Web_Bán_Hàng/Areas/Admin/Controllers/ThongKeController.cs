using Microsoft.AspNetCore.Mvc;
using Web_Ban_Hang.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;

namespace Web_Ban_Hang.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ThongKeController : Controller
    {
        private readonly Datacontext _datacontext;

        public ThongKeController(Datacontext datacontext)
        {
            _datacontext = datacontext;
        }

        public IActionResult Index()
        {
            // Lay tong doanh thu, tong don hang, tong san pham ban ra
            var tongDoanhThu = _datacontext.DonHangs.Sum(dh => dh.TongTienCuoi);
            var tongDonHang = _datacontext.DonHangs.Count();
            var tongSanPhamBanRa = _datacontext.ChiTietDonHangs.Sum(ct => ct.Soluong);

            // Tinh san pham ban chay nhat
            var sanPhamBanChayNhat = _datacontext.ChiTietDonHangs
                .GroupBy(ct => ct.MaSanPham)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(ct => ct.Soluong)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .FirstOrDefault();

            // Lay ten san pham ban chay nhat tu bang ProductModel
            var tenSanPhamBanChayNhat = sanPhamBanChayNhat != null
                ? _datacontext.Products.FirstOrDefault(p => p.Id == sanPhamBanChayNhat.ProductId)?.Name
                : "Khong co san pham ban chay";
            var tongSanPham = _datacontext.Products.Count();

            var luotDanhGia = _datacontext.DanhGias.Count();

            var viewModel = new ThongKeViewModel
            {
                TongDoanhThu = tongDoanhThu,
                TongDonHang = tongDonHang,
                TongSanPhamBanRa = tongSanPhamBanRa,
                SanPhamBanChayNhat = tenSanPhamBanChayNhat,
                TongSanPham = tongSanPham, 
                LuotDanhGia = luotDanhGia, 

                // Lay du lieu bieu do
                DuLieuBieuDoDoanhThu = GetRevenueChartData(),
                DuLieuBieuDoDonHang = GetOrderChartData()
            };

            return View(viewModel);
        }

        // Lay du lieu bieu do doanh thu theo ngay/thang/nam
        private List<DuLieuBieuDoDoanhThu> GetRevenueChartData()
        {
            var duLieuDoanhThu = _datacontext.DonHangs
                .GroupBy(dh => dh.NgayDat.Date) // Nhom theo ngay
                .Select(g => new DuLieuBieuDoDoanhThu
                {
                    Ngay = g.Key.ToString("yyyy-MM-dd"), // Dinh dang ngay theo yyyy-MM-dd
                    DoanhThu = g.Sum(dh => dh.TongTienCuoi) // Tong doanh thu theo ngay
                }).ToList();

            return duLieuDoanhThu;
        }

        // Lay du lieu bieu do so hoa don theo ngay/thang/nam
        private List<DuLieuBieuDoDonHang> GetOrderChartData()
        {
            var duLieuDonHang = _datacontext.DonHangs
                .GroupBy(dh => dh.NgayDat.Date) // Nhom theo ngay
                .Select(g => new DuLieuBieuDoDonHang
                {
                    Ngay = g.Key.ToString("yyyy-MM-dd"), // Dinh dang ngay theo yyyy-MM-dd
                    SoDonHang = g.Count() // So don hang theo ngay
                }).ToList();

            return duLieuDonHang;
        }
    }
}
