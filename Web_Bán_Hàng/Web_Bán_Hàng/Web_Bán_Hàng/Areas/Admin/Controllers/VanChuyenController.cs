using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class VanChuyenController : Controller
    {
        private readonly Datacontext _datacontext;

        public VanChuyenController(Datacontext context)
        {
            _datacontext = context;
        }

        public async Task<IActionResult> Index()
        {
            var listdiachi = await _datacontext.VanChuyens.ToListAsync();  
            ViewBag.List = listdiachi;  
            return View(); 
        }
        [HttpPost]
        public async Task<IActionResult> Add(VanChuyenModel vanChuyen, string phuong, string quan, string tinh, decimal gia)
        {
            // Gán các giá trị từ tham số vào đối tượng ShippingModel
            vanChuyen.thanhpho = tinh;
            vanChuyen.Quan = quan;
            vanChuyen.Phuong_xa = phuong;
            vanChuyen.Gia = gia;

            try
            {
                // Kiểm tra dữ liệu trùng lặp
                var vanchuyentontai = await _datacontext.VanChuyens.AnyAsync(x => x.thanhpho == tinh && x.Quan == quan && x.Phuong_xa == phuong);


                if (vanchuyentontai)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp." });
                }

                // Thêm mới vào database
                _datacontext.VanChuyens.Add(vanChuyen);
                await _datacontext.SaveChangesAsync();

                return Ok(new { success = true, message = "Thêm địa chỉ vận chuyển thành công." });
            }
            catch (Exception )
            {
       
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi thêm địa chỉ vận chuyển." });
            }
        }
        public async Task<IActionResult> Delete(int Id)
        {
            // Tìm đối tượng Shipping bằng Id
            var vanchuyen = await _datacontext.VanChuyens.FindAsync(Id);

            // Kiểm tra xem đối tượng có tồn tại không
            if (vanchuyen == null)
            {
                // Nếu không tìm thấy đối tượng, trả về thông báo lỗi hoặc hành động khác
                TempData["error"] = "Không tìm thấy đối tượng vận chuyển.";
                return RedirectToAction("Index");
            }

            // Xóa đối tượng Shipping
            _datacontext.VanChuyens.Remove(vanchuyen);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _datacontext.SaveChangesAsync();

            // Đặt thông báo thành công vào TempData
            TempData["success"] = "Shipping đã được xóa thành công";

            // Redirect về trang Index sau khi xóa
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm địa chỉ vận chuyển theo ID
            var vanchuyen = await _datacontext.VanChuyens.FindAsync(id);
            if (vanchuyen == null)
            {
                // Nếu không tìm thấy, trả về thông báo lỗi hoặc chuyển hướng
                TempData["error"] = "Không tìm thấy địa chỉ vận chuyển.";
                return RedirectToAction("Index");
            }

            // Trả về view và truyền đối tượng VanChuyenModel để hiển thị lên form
            return View(vanchuyen);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, VanChuyenModel updatedVanChuyen)
        {
            //Kiêm tra id đc gửi có trùng vs id của view form không
            if (id != updatedVanChuyen.Id)
            {
                TempData["error"] = "ID không khớp.";
                return RedirectToAction("Index");
            }

            try
            {
                // Tìm địa chỉ vận chuyển cần sửa
                var vanchuyen = await _datacontext.VanChuyens.FindAsync(id);
                if (vanchuyen == null)
                {
                    // Nếu không tìm thấy địa chỉ, trả về thông báo lỗi
                    TempData["error"] = "Không tìm thấy địa chỉ vận chuyển.";
                    return RedirectToAction("Index");
                }

                // Cập nhật giá trị mới từ form
                vanchuyen.Gia = updatedVanChuyen.Gia;

                // Cập nhật các thay đổi vào cơ sở dữ liệu
                _datacontext.VanChuyens.Update(vanchuyen);
                await _datacontext.SaveChangesAsync();

                // Thông báo thành công
                TempData["success"] = "Địa chỉ vận chuyển đã được cập nhật thành công.";

                // Chuyển hướng về trang Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                TempData["error"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }



    }
}
