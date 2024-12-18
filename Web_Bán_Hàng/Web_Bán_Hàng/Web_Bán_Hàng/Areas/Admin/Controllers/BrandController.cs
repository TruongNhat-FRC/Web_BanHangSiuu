using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
	[Area("Admin")]
    
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
	{
		private readonly Datacontext _datacontext;

		public BrandController(Datacontext context)
		{
			_datacontext = context;
		}

        public async Task<IActionResult> Index(int pg = 1)
        {
            const int pageSize = 10; // Số lượng thương hiệu mỗi trang

            // Lấy danh sách các thương hiệu và tổng số lượng
            var brandsQuery = _datacontext.Brands.OrderByDescending(p => p.Id);
            var totalBrands = await brandsQuery.CountAsync();

			// Tính toán phân trang
			var paging = new PhanTrang(totalBrands, pg, pageSize);
            var skip = (pg - 1) * pageSize;

            // Lấy danh sách thương hiệu cho trang hiện tại
            var brands = await brandsQuery.Skip(skip).Take(pageSize).ToListAsync();

            // Truyền thông tin phân trang và danh sách thương hiệu vào View
            ViewBag.Trang = paging;

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

        public async Task<IActionResult> Edit(int id,BrandModel brand)
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
                    ModelState.AddModelError("", "Thương Hiệu đã tồn tại");
                    return View(brand);
                }

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
            return View(brand);

        }
    }
}
