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

            var tenSanPhamBanChayNhat = sanPhamBanChayNhat != null
                ? _datacontext.Products.FirstOrDefault(p => p.Id == sanPhamBanChayNhat.ProductId)?.Name
                : "Khong co san pham ban chay";
            var tongSanPham = _datacontext.Products.Count();

            var luotDanhGia = _datacontext.DanhGias.Count();
            var sanPhamHot = _datacontext.Products
                                          .Where(p => p.IsVisible)
                                          .OrderByDescending(p => p.PurchaseCount)
                                          .Take(10)
                                          .ToList();



            var viewModel = new ThongKeViewModel
            {
                TongDoanhThu = tongDoanhThu,
                TongDonHang = tongDonHang,
                TongSanPhamBanRa = tongSanPhamBanRa,
                SanPhamBanChayNhat = tenSanPhamBanChayNhat,
                TongSanPham = tongSanPham, 
                LuotDanhGia = luotDanhGia, 

                
                DuLieuBieuDoDoanhThu = GetRevenueChartData(),
                DuLieuBieuDoDonHang = GetOrderChartData(),
                BanChay = sanPhamHot
            };
           

            return View(viewModel);
        }

        private List<DuLieuBieuDoDoanhThu> GetRevenueChartData()
        {
            var duLieuDoanhThu = _datacontext.DonHangs
                .GroupBy(dh => dh.NgayDat.Date) 
                .Select(g => new DuLieuBieuDoDoanhThu
                {
                    Ngay = g.Key.ToString("yyyy-MM-dd"),
                    DoanhThu = g.Sum(dh => dh.TongTienCuoi) 
                }).ToList();

            return duLieuDoanhThu;
        }

    
        private List<DuLieuBieuDoDonHang> GetOrderChartData()
        {
            var duLieuDonHang = _datacontext.DonHangs
                .GroupBy(dh => dh.NgayDat.Date) 
                .Select(g => new DuLieuBieuDoDonHang
                {
                    Ngay = g.Key.ToString("yyyy-MM-dd"), 
                    SoDonHang = g.Count() 
                }).ToList();

            return duLieuDonHang;
        }
    }
}
