using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DonHangController : Controller
    {
		private readonly Datacontext _datacontext;
		public DonHangController(Datacontext datacontext)
		{
			_datacontext = datacontext;
		}

		public async Task<IActionResult> Index()
        {
            return View(await _datacontext.DonHangs.OrderByDescending(p=>p.ID).ToListAsync());
        }
        public async Task<IActionResult> Details(string madonhang)
        {
            var donHang = await _datacontext.DonHangs
                .Include(d => d.ChiTietDonHangs) // Bao gồm các chi tiết đơn hàng
                .ThenInclude(ct => ct.Product) // Bao gồm thông tin sản phẩm (nếu có)
                .FirstOrDefaultAsync(d => d.MaDonHang == madonhang);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
        
        
        public async Task<IActionResult> Delete(string madonhang)
        {
            var donHang = await _datacontext.DonHangs
                .Include(d => d.ChiTietDonHangs) // Bao gồm các chi tiết liên quan
                .FirstOrDefaultAsync(d => d.MaDonHang == madonhang);

            if (donHang == null)
            {
                return NotFound();
            }

            // Xóa các chi tiết đơn hàng liên quan trước
            _datacontext.ChiTietDonHangs.RemoveRange(donHang.ChiTietDonHangs);

            // Xóa đơn hàng
            _datacontext.DonHangs.Remove(donHang);

            await _datacontext.SaveChangesAsync();
            return RedirectToAction("Index", "DonHang", new { area = "Admin" }); // Quay về danh sách đơn hàng
        }
		[HttpGet]
		public async Task<IActionResult> Edit(string madonhang)
		{
			var donHang = await _datacontext.DonHangs
				.FirstOrDefaultAsync(d => d.MaDonHang == madonhang);

			if (donHang == null)
			{
				return NotFound();
			}

			var donHangModel = new DonHangModel
			{
				ID = donHang.ID,
				MaDonHang = donHang.MaDonHang,
				MaNguoiDung = donHang.MaNguoiDung,
				NgayDat = donHang.NgayDat,
				TrangThai = donHang.TrangThai,
				TongTienCuoi = donHang.TongTienCuoi
			};

			return View(donHangModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DonHangModel donHangModel)
		{
			if (ModelState.IsValid)
			{
				var donHang = await _datacontext.DonHangs
					.FirstOrDefaultAsync(d => d.MaDonHang == donHangModel.MaDonHang);

				if (donHang != null)
				{
					donHang.MaNguoiDung = donHangModel.MaNguoiDung;
					donHang.NgayDat = donHangModel.NgayDat;
					donHang.TrangThai = donHangModel.TrangThai;
					donHang.TongTienCuoi = donHangModel.TongTienCuoi;

					_datacontext.Update(donHang);
					await _datacontext.SaveChangesAsync();

					return RedirectToAction(nameof(Index));
				}
			}
			return View(donHangModel);
		}



	}
}
