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
    [Authorize(Roles = "Admin,NhanVien")]

    public class CategoryController : Controller
    {
        private readonly Datacontext _datacontext;

        public CategoryController(Datacontext context, IWebHostEnvironment webhostenvironment)
        {
            _datacontext = context;

        }
        public async Task<IActionResult> Index()
        {
            // Lấy toàn bộ danh sách danh mục từ cơ sở dữ liệu
            List<CategoryModel> danhmuc = await _datacontext.Categories.ToListAsync();

            // Truyền danh sách danh mục vào View
            return View(danhmuc);
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
                return RedirectToAction("Index"); 
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
				if (string.IsNullOrEmpty(category.Slug))
				{
					category.Slug = category.Name.Replace(" ", "_").ToLower();
				}

				var slug = await _datacontext.Categories
					.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.Id != category.Id);

				if (slug != null)
				{
					ModelState.AddModelError("", "Danh Mục đã tồn tại");
					return View(category);
				}
                
				// Cập nhật thông tin danh mục
				_datacontext.Update(category);
				await _datacontext.SaveChangesAsync();

				TempData["success"] = "Cập nhật thành công";
				return RedirectToAction("Index", "Category");
			}
			else
			{
				TempData["error"] = "Vui lòng kiểm tra lại dữ liệu";
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
