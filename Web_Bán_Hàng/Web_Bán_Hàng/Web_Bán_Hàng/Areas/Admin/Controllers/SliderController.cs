using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    {
      
        private readonly Datacontext _datacontext;
        private readonly IWebHostEnvironment _webhostenvironment;

        public SliderController(Datacontext context, IWebHostEnvironment webhostenvironment)
        {
            _datacontext = context;
            _webhostenvironment = webhostenvironment;

        }
        public async Task<IActionResult> Index()
        {
            return View(await _datacontext.Sliders.OrderByDescending(p=>p.Id).ToListAsync());
        }
        public async Task<IActionResult> Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SliderModel slider)
        {
            
            if (ModelState.IsValid)
            {
                
                
                if (slider.ImageUpLoad != null)
                {
                    string upload = Path.Combine(_webhostenvironment.WebRootPath, "slide");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpLoad.FileName;
                    string filePath = Path.Combine(upload, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpLoad.CopyToAsync(fs);
                    fs.Close();
                    slider.Image = imageName;
                }
                _datacontext.Add(slider);
                await _datacontext.SaveChangesAsync();
                TempData["success"] = "Thêm slide mới thành công";
                return RedirectToAction("Index");
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
            return View(slider);

        }
        public async Task<IActionResult> Edit(int id)
        {
            SliderModel slide = await _datacontext.Sliders.FindAsync(id);
            

            return View(slide);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SliderModel slider)
        {


            // Tìm sản phẩm trong cơ sở dữ liệu
            var sp_da_ton_tai = await _datacontext.Sliders.FindAsync(slider.Id);
            if (sp_da_ton_tai == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            if (ModelState.IsValid)
            {
               

            

                // Xử lý upload hình ảnh nếu có
                if (slider.ImageUpLoad != null)
                {
                    string upload = Path.Combine(_webhostenvironment.WebRootPath, "slide");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpLoad.FileName;
                    string filePath = Path.Combine(upload, imageName);

                    // Xóa hình ảnh cũ nếu tồn tại
                    string anh_cu = Path.Combine(upload, sp_da_ton_tai.Image);
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
                        return View(slider);
                    }

                    // Lưu hình ảnh mới
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await slider.ImageUpLoad.CopyToAsync(fs);
                    }
                    sp_da_ton_tai.Image = imageName;
                }

                // Cập nhật các trường khác
                sp_da_ton_tai.Name = slider.Name;
                sp_da_ton_tai.Mota = slider.Mota;
               sp_da_ton_tai.Status = slider.Status;
               
             

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
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm slider theo Id
            SliderModel slider = await _datacontext.Sliders.FindAsync(id);

            if (slider == null)
            {
                TempData["error"] = "Slider không tồn tại.";
                return RedirectToAction("Index");
            }

            // Đường dẫn đến file cũ
            string uploadsDir = Path.Combine(_webhostenvironment.WebRootPath, "slide");
            string oldFilePath = Path.Combine(uploadsDir, slider.Image);

            // Kiểm tra và xóa file cũ nếu tồn tại
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Xóa slider khỏi cơ sở dữ liệu
            _datacontext.Sliders.Remove(slider);
            await _datacontext.SaveChangesAsync();

            // Thông báo thành công
            TempData["success"] = "Slider đã được xóa thành công.";
            return RedirectToAction("Index");
        }

    }
}
