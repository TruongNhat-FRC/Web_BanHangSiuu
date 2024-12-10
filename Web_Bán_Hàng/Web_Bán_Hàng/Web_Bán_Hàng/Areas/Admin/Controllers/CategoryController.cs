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

    public class CategoryController : Controller
    {
        private readonly Datacontext _datacontext;

        public CategoryController(Datacontext context, IWebHostEnvironment webhostenvironment)
        {
            _datacontext = context;

        }
        public  async Task<IActionResult> Index(int pg = 1) // Thay vì tr = 1
        {
            List<CategoryModel> danhmuc = _datacontext.Categories.ToList();
            const int KichThuoc = 10;
            if (pg < 1)
            {
                pg = 1;
            }
            int count = danhmuc.Count();
            var trang = new PhanTrang(count, pg, KichThuoc);
            int buocnhay = (pg - 1) * KichThuoc;
            var data = danhmuc.Skip(buocnhay).Take(trang.KichThuocTrang).ToList();
            ViewBag.Trang = trang;
            return View(data);
        }

        public IActionResult Add()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Add(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra và gán Slug nếu chưa có giá trị
                if (string.IsNullOrEmpty(category.Slug))
                {
                    category.Slug = category.Name.Replace(" ", "_").ToLower();
                }
                var slug = await _datacontext.Products.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại");
                    return View(category);
                }

                _datacontext.Add(category);
                await _datacontext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
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


            return View(category);

        }
        public async Task<IActionResult> Delete(int id)
        {
            CategoryModel category = await _datacontext.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách sản phẩm
            }

            _datacontext.Categories.Remove(category);
            await _datacontext.SaveChangesAsync();
            TempData["success"] = " Đã xóa danh mục thành công";
            return RedirectToAction("Index", "Category");
        }
        public async Task<IActionResult> Edit(int id)
        {
            CategoryModel category = await _datacontext.Categories.FindAsync(id);


            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {

            if (ModelState.IsValid)
            {
                // Kiểm tra và gán Slug nếu chưa có giá trị
                if (string.IsNullOrEmpty(category.Slug))
                {
                    category.Slug = category.Name.Replace(" ", "_").ToLower();
                }
                var slug = await _datacontext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh Mục đã tồn tại");
                    return View(category);
                }

                _datacontext.Update(category);
                await _datacontext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thành công";
                return RedirectToAction("Index", "Category");
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
            return View(category);

        }
    }
}
