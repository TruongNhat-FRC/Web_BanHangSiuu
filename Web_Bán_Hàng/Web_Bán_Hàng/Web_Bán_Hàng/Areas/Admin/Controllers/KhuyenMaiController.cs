using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class KhuyenMaiController : Controller
    {
        private readonly Datacontext _datacontext;

        public KhuyenMaiController(Datacontext context)
        {
            _datacontext = context;
        }
        public async Task<IActionResult> Index()
        {
            var khuyenmai = await _datacontext.KhuyenMais.ToListAsync();
            ViewBag.KhuyenMai = khuyenmai;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Add(KhuyenMaiModel khuyenmai)
        {
            if (ModelState.IsValid)
            {
                _datacontext.Add(khuyenmai);
                await _datacontext.SaveChangesAsync();
                TempData["success"] = "Thêm mã khuyến mãi thành công";
                return RedirectToAction("Index", "Brand");
            }
            else
            {
                TempData["error"] = "Vui lòng kiểm tra lại dữ liệu ";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }


            return View(khuyenmai);

        }
        public async Task<IActionResult> Delete(int Id)
        {
            // Tìm đối tượng Shipping bằng Id
            var khuyenMai = await _datacontext.KhuyenMais.FindAsync(Id);

            // Kiểm tra xem đối tượng có tồn tại không
            if (khuyenMai == null)
            {
                // Nếu không tìm thấy đối tượng, trả về thông báo lỗi hoặc hành động khác
                TempData["error"] = "Không tìm thấy mã khuyễn mãi.";
                return RedirectToAction("Index");
            }

            // Xóa đối tượng Shipping
            _datacontext.KhuyenMais.Remove(khuyenMai);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _datacontext.SaveChangesAsync();

            // Đặt thông báo thành công vào TempData
            TempData["success"] = "Mã Giảm Giá Đã Được Xóa Thành Công";

            // Redirect về trang Index sau khi xóa
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int Id)
        {
            var khuyenmai = await _datacontext.KhuyenMais.FindAsync(Id);
            if (khuyenmai == null)
            {
                TempData["error"] = "Không Tìm Thấy Mã Khuyến Mãi.";
                return RedirectToAction("Index");
            }

            return View(khuyenmai);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, KhuyenMaiModel khuyenmaiview)
        {
            if (Id != khuyenmaiview.Id)
            {
                TempData["error"] = "Mã khuyến mãi không đúng.";
                return RedirectToAction("Index");
            }

            try
            {
                var khuyenmai = await _datacontext.KhuyenMais.FindAsync(Id);
                if (khuyenmai == null)
                {
                    TempData["error"] = "Không Thể Tìm Thấy Mã Khuyến Mãi.";
                    return RedirectToAction("Index");
                }
                khuyenmai.Mota = khuyenmaiview.Mota;
                khuyenmai.Gia = khuyenmaiview.Gia;
                khuyenmai.SoLuong = khuyenmaiview.SoLuong;
                khuyenmai.NgayBatDau = khuyenmaiview.NgayBatDau;
                khuyenmai.NgayKetThuc = khuyenmaiview.NgayKetThuc;
              

                _datacontext.KhuyenMais.Update(khuyenmai);
                await _datacontext.SaveChangesAsync();

                TempData["success"] = "Mã Khuyến Mãi Đã Được Cập Nhật Thành Công.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
