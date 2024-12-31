using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;
using Web_Bán_Hàng.Models.ViewModel;

namespace Web_Bán_Hàng.Controllers
{
	public class ProductController : Controller
	{
		private readonly Datacontext _datacontext;
		
		public ProductController (Datacontext context)
		{
			_datacontext = context;
		}
		public IActionResult Index()
		{
			return View();
		}
		
		public async Task<IActionResult> Detail(string Id = "")
		{
			if (string.IsNullOrEmpty(Id)) return RedirectToAction("Index");

			// Lấy sản phẩm theo Id
			var productbyId = await _datacontext.Products
				.Include(p => p.DanhGias) // Lấy các đánh giá liên quan
				.FirstOrDefaultAsync(p => p.Id == Id);

			if (productbyId == null) return RedirectToAction("Index");

			ViewBag.Name = productbyId.Name;

			var danhGiasSorted = productbyId.DanhGias
				.OrderByDescending(dg => dg.CreatedAt) 
				.ToList(); // Lấy tất cả đánh giá

			
			var viewModel = new ProductDanhGiaModel
			{
				ProductDetail = productbyId,
				DanhGias = danhGiasSorted
			};

			var sanphamlienquan = await _datacontext.Products
				.Where(p => p.CategoryId == productbyId.CategoryId
							&& p.Id != productbyId.Id
							&& p.IsVisible) 
				.Take(10)
				.ToListAsync();

			ViewBag.SanPhamLienquan = sanphamlienquan;

			return View(viewModel);
		}


		[HttpPost]
        [HttpGet]
        public async Task<IActionResult> TimKiem(string searchkey, int pg = 1)
        {
            if (string.IsNullOrWhiteSpace(searchkey))
            {
                return RedirectToAction("Index"); // Quay về trang chính nếu không có từ khóa
            }

            var products = await _datacontext.Products
                                             .Where(p => p.Name.Contains(searchkey) || p.Description.Contains(searchkey))
                                             .ToListAsync();

            var sanPhamHot = _datacontext.Products
                                          .Where(p => p.IsVisible)
                                          .OrderByDescending(p => p.PurchaseCount)
                                          .Take(10)
                                          .ToList();

            ViewBag.SanPhamHot = sanPhamHot;
            ViewBag.Keyword = searchkey;

            const int KichThuoc = 6; // Số sản phẩm mỗi trang
            if (pg < 1)
            {
                pg = 1;
            }

            int count = products.Count();
            var trang = new PhanTrang(count, pg, KichThuoc);
            int buocnhay = (pg - 1) * KichThuoc;
            var data = products.Skip(buocnhay).Take(trang.KichThuocTrang).ToList();

            ViewBag.Trang = trang;

            return View(data);
        }


        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DanhGia(DanhGia danhGia)
		{
			// Kiểm tra nếu người dùng chưa đăng nhập
			if (!User.Identity.IsAuthenticated)
			{
				TempData["error"] = "Vui lòng đăng nhập để bình luận.";
				return RedirectToAction("DangNhap", "Account"); // Hoặc chuyển hướng đến trang đăng nhập
			}

			// Lấy thông tin người dùng từ User
			string userName = User.Identity.Name; // Tên đăng nhập
			string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value; // Email

			if (ModelState.IsValid)
			{
				var danh_gia = new DanhGia
				{
					ProductId = danhGia.ProductId,
					NameUser = userName ?? "Khách hàng", // Gán giá trị NameUser
					Email = email ?? danhGia.Email, // Gán Email, nếu không có lấy từ form
					BinhLuan = danhGia.BinhLuan,
					Diem = danhGia.Diem,
					CreatedAt = DateTime.Now,
				};
				_datacontext.DanhGias.Add(danh_gia);
				await _datacontext.SaveChangesAsync();
				TempData["success"] = "Đã đăng bình luận thành công";
				return Redirect(Request.Headers["Referer"]);
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
				return RedirectToAction("Detail", new { id = danhGia.ProductId });
			}
		}
        //Show full
        public async Task<IActionResult> Showfull(string Id, int pg = 1)
        {
            if (string.IsNullOrEmpty(Id)) return RedirectToAction("Index");

            var productbyId = await _datacontext.Products
                .Include(p => p.DanhGias)
                .Where(p => p.Id == Id)
                .FirstOrDefaultAsync();

            if (productbyId == null) return RedirectToAction("Index");

            const int pageSize = 7; // Số lượng đánh giá mỗi trang
            var totalDanhGias = productbyId.DanhGias.Count(); // Tổng số đánh giá

            // Tính toán phân trang
            var paging = new PhanTrang(totalDanhGias, pg, pageSize);
            var skip = (pg - 1) * pageSize;

            // Lấy danh sách đánh giá cho trang hiện tại
            var danhGias = productbyId.DanhGias.OrderByDescending(d => d.CreatedAt)
                                               .Skip(skip)
                                               .Take(pageSize)
                                               .ToList();

            // Tạo model để gửi dữ liệu qua view
            var viewModel = new ProductDanhGiaModel
            {
                ProductDetail = productbyId,
                DanhGias = danhGias
            };

            // Truyền thông tin phân trang vào View
            ViewBag.Paging = paging;

            return View(viewModel);
        }




    }
}
