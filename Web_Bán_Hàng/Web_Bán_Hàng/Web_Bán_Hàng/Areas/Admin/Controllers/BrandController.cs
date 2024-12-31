using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
	[Area("Admin")]
    
    [Authorize(Roles = "Admin,NhanVien")]
    public class BrandController : Controller
	{
		private readonly Datacontext _datacontext;

		public BrandController(Datacontext context)
		{
			_datacontext = context;
		}

        public async Task<IActionResult> Index()
        {
            // Lấy toàn bộ danh sách thương hiệu từ cơ sở dữ liệu
            var brands = await _datacontext.Brands.OrderByDescending(p => p.Id).ToListAsync();

            // Truyền danh sách thương hiệu vào View
            return View(brands);
        }


        public IActionResult Add()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Add(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra và gán Slug nếu chưa có giá trị
                if (string.IsNullOrEmpty(brand.Slug))
                {
                    brand.Slug = brand.Name.Replace(" ", "_").ToLower();
                }
                var slug = await _datacontext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã tồn tại");
                    return View(brand);
                }

                _datacontext.Add(brand);
                await _datacontext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
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


            return View(brand);

        }
        public async Task<IActionResult> Delete(int id)
        {
            BrandModel brand = await _datacontext.Brands.FindAsync(id);
            if (brand == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách sản phẩm
            }

            _datacontext.Brands.Remove(brand);
            await _datacontext.SaveChangesAsync();
            TempData["success"] = " Đã xóa danh mục thành công";
            return RedirectToAction("Index", "Brand");
        }
        public async Task<IActionResult> Edit(int id)
        {
            BrandModel brand = await _datacontext.Brands.FindAsync(id);


            return View(brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

		public async Task<IActionResult> Edit(int id, BrandModel brand)
		{
			if (ModelState.IsValid)
			{
				// Kiểm tra và gán Slug nếu chưa có giá trị
				if (string.IsNullOrEmpty(brand.Slug))
				{
					brand.Slug = brand.Name.Replace(" ", "_").ToLower();
				}

				// Kiểm tra nếu Slug đã tồn tại và không phải là của chính thương hiệu hiện tại
				var existingSlug = await _datacontext.Brands
					.FirstOrDefaultAsync(p => p.Slug == brand.Slug && p.Id != brand.Id);

				if (existingSlug != null)
				{
					ModelState.AddModelError("", "Thương Hiệu đã tồn tại");
					return View(brand);
				}

				// Cập nhật thông tin thương hiệu
				_datacontext.Update(brand);
				await _datacontext.SaveChangesAsync();

				TempData["success"] = "Cập nhật thành công";
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
		}

	}
}
