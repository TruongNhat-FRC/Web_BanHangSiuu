using Microsoft.AspNetCore;
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

    public class ProductController : Controller
	{
		private readonly Datacontext _datacontext;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(Datacontext context,IWebHostEnvironment webhostenvironment )
		{
			_datacontext = context;
			_webHostEnvironment = webhostenvironment;

		}
        public async Task<IActionResult> Index(int pg = 1)
        {
            const int pageSize = 10; // Số lượng sản phẩm mỗi trang
            var productsQuery = _datacontext.Products.OrderByDescending(p => p.Id)
                                                     .Include(p => p.Category)
                                                     .Include(p => p.Brand);

            var totalProducts = await productsQuery.CountAsync(); // Tổng số sản phẩm

            // Tính toán phân trang
            var paging = new PhanTrang(totalProducts, pg, pageSize);
            var skip = (pg - 1) * pageSize;

            // Lấy danh sách sản phẩm cho trang hiện tại
            var products = await productsQuery.Skip(skip).Take(pageSize).ToListAsync();

            // Truyền thông tin phân trang và danh sách sản phẩm vào View
            ViewBag.Paging = paging;

            return View(products);
        }


        public IActionResult Add()
		{
			ViewBag.Categories = new SelectList(_datacontext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_datacontext.Brands, "Id", "Name");
            return View();
		}
        [HttpPost]
		[ValidateAntiForgeryToken]
       
		public async Task<IActionResult> Add(ProductModel product)
		{
            ViewBag.Categories = new SelectList(_datacontext.Categories, "Id", "Name",product.CategoryId);
            ViewBag.Brands = new SelectList(_datacontext.Brands, "Id", "Name", product.BrandId);
			if (ModelState.IsValid)
			{
                // Kiểm tra và gán Slug nếu chưa có giá trị
                if (string.IsNullOrEmpty(product.Slug))
                {
                    product.Slug = product.Name.Replace(" ", "_").ToLower();
                }
                var slug = await _datacontext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm đã tồn tại");
					return View(product);
				}
				if(product.ImageUpLoad != null)
				{
					string upload = Path.Combine(_webHostEnvironment.WebRootPath,"image");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpLoad.FileName;
					string filePath = Path.Combine(upload, imageName);
					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpLoad.CopyToAsync(fs);
					fs.Close();
					product.Image = imageName;
				}
				_datacontext.Add(product);
				await   _datacontext.SaveChangesAsync();
				TempData["success"] = "Thêm sản phẩm thành công";
				return RedirectToAction("Index");
            }
            else
			{
				TempData["error"] = "Vui lòng kiểm tra lại dữ liệu ";
				List<string> errors = new List<string>();
				foreach(var value in ModelState.Values)
				{
					foreach(var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
                string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
            }
            return View(product);

        }
		public async Task<IActionResult> Edit(string id)
		{
			ProductModel product = await _datacontext.Products.FindAsync(id);
            ViewBag.Categories = new SelectList(_datacontext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_datacontext.Brands, "Id", "Name");

            return View(product);
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, ProductModel product)
		{
			// Gán danh mục và thương hiệu vào ViewBag để hiển thị trên View
			ViewBag.Categories = new SelectList(_datacontext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_datacontext.Brands, "Id", "Name", product.BrandId);

			// Tìm sản phẩm trong cơ sở dữ liệu
			var sp_da_ton_tai = await _datacontext.Products.FindAsync(product.Id);
			if (sp_da_ton_tai == null)
			{
				return NotFound("Không tìm thấy sản phẩm.");
			}

			if (ModelState.IsValid)
			{
				// Kiểm tra và tạo slug nếu chưa có giá trị
				if (string.IsNullOrEmpty(product.Slug))
				{
					product.Slug = product.Name.Replace(" ", "_").ToLower();
				}

				// Kiểm tra slug có bị trùng không (bỏ qua sản phẩm hiện tại)
				var slug = await _datacontext.Products
					.FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != product.Id);
				if (slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm đã tồn tại với Slug này.");
					return View(product);
				}

				// Xử lý upload hình ảnh nếu có
				if (product.ImageUpLoad != null)
				{
					string upload = Path.Combine(_webHostEnvironment.WebRootPath, "image");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpLoad.FileName;
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
						return View(product);
					}

					// Lưu hình ảnh mới
					using (var fs = new FileStream(filePath, FileMode.Create))
					{
						await product.ImageUpLoad.CopyToAsync(fs);
					}
					sp_da_ton_tai.Image = imageName;
				}

				// Cập nhật các trường khác
				sp_da_ton_tai.Name = product.Name;
				sp_da_ton_tai.Description = product.Description;
				sp_da_ton_tai.Price = product.Price;
				sp_da_ton_tai.BrandId = product.BrandId;
				sp_da_ton_tai.CategoryId = product.CategoryId;
				sp_da_ton_tai.Slug = product.Slug;
				sp_da_ton_tai.Quantity = product.Quantity;

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


        /*public async Task<IActionResult> Edit( string id ,ProductModel product)
        {
            ViewBag.Categories = new SelectList(_datacontext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_datacontext.Brands, "Id", "Name", product.BrandId);
            var sp_da_ton_tai = _datacontext.Products.Find(product.Id);
            if (ModelState.IsValid)
            {
                // Kiểm tra và gán Slug nếu chưa có giá trị
                if (string.IsNullOrEmpty(product.Slug))
                {
                    product.Slug = product.Name.Replace(" ", "_").ToLower();
                }
                var slug = await _datacontext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã tồn tại");
                    return View(product);
                }
                if (product.ImageUpLoad != null)
                {
                    string upload = Path.Combine(_webHostEnvironment.WebRootPath, "image");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpLoad.FileName;
                    string filePath = Path.Combine(upload, imageName);
                    //Xóa ảnh cũ
                    string anh_cu = Path.Combine(upload, sp_da_ton_tai.Image);

                    try
                    {
                        if (System.IO.File.Exists(anh_cu))
                        {
                            System.IO.File.Delete(anh_cu);
                        }

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Có lỗi trong việc xóa hình ảnh ");
                    }
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpLoad.CopyToAsync(fs);
                    fs.Close();
                 
                    sp_da_ton_tai.Image = imageName;
                }


                sp_da_ton_tai.Name = product.Name;
                sp_da_ton_tai.Description = product.Description;
                sp_da_ton_tai.Price = product.Price;
                sp_da_ton_tai.BrandId = product.BrandId;
                sp_da_ton_tai.CategoryId = product.CategoryId;



                _datacontext.Update(sp_da_ton_tai);
                await _datacontext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thành công";
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
            return View(product);

        }*/
        public async Task<IActionResult> Delete(string id)
        {
            ProductModel product = await _datacontext.Products.FindAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách sản phẩm
            }

            try
            {
                product.IsVisible = false; // Cập nhật trạng thái hiển thị
                _datacontext.Products.Update(product);
                await _datacontext.SaveChangesAsync();
                TempData["success"] = "Đã xóa sản phẩm thành công";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi xóa sản phẩm: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

    }

}
