using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class LienHeController : Controller
    {
        private readonly Datacontext _datacontext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LienHeController(Datacontext context, IWebHostEnvironment webhostenvironment)
        {
            _datacontext = context;
            _webHostEnvironment = webhostenvironment;

        }
        public async Task<IActionResult> Index()
        {
            var lienhe = await _datacontext.LienHes.ToListAsync();
            return View(lienhe);
        }
        public async Task<IActionResult> Edit(int id)
        {
            LienHeModel lienhe = await _datacontext.LienHes.FindAsync(id);
            return View(lienhe);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LienHeModel lienHe)
        {
            // Gán danh mục và thương hiệu vào ViewBag để hiển thị trên View
      

            // Tìm sản phẩm trong cơ sở dữ liệu
            var sp_da_ton_tai = await _datacontext.LienHes.FindAsync(id);
            if (sp_da_ton_tai == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            if (ModelState.IsValid)
            {
              

                // Xử lý upload hình ảnh nếu có
                if (lienHe.ImageUpload != null)
                {
                    string upload = Path.Combine(_webHostEnvironment.WebRootPath, "logo");
                    string imageName = Guid.NewGuid().ToString() + "_" + lienHe.ImageUpload.FileName;
                    string filePath = Path.Combine(upload, imageName);

                    // Xóa hình ảnh cũ nếu tồn tại
                    string anh_cu = Path.Combine(upload, sp_da_ton_tai.Logo);
                    try
                    {
                        if (System.IO.File.Exists(anh_cu))
                        {
                            System.IO.File.Delete(anh_cu);
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Có lỗi trong việc xóa hình ảnh.");
                        return View(lienHe);
                    }

                    // Lưu hình ảnh mới
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await lienHe.ImageUpload.CopyToAsync(fs);
                    }
                    sp_da_ton_tai.Logo = imageName;
                }

                // Cập nhật các trường khác
                sp_da_ton_tai.Name = lienHe.Name;
                sp_da_ton_tai.BanDo = lienHe.BanDo;
                sp_da_ton_tai.Email= lienHe.Email;
                sp_da_ton_tai.SDT = lienHe.SDT;
                sp_da_ton_tai.Mota = lienHe.Mota; 
               
                // Lưu thay đổi vào cơ sở dữ liệu
                _datacontext.Update(sp_da_ton_tai);
                await _datacontext.SaveChangesAsync();

                TempData["success"] = "Cập nhật sản phẩm thành công.";
                return RedirectToAction("Index");
            }

            // Xử lý nếu ModelState không hợp lệ
            TempData["error"] = "Vui lòng kiểm tra lại dữ liệu.";
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
    }
}
