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
		/*public async Task<IActionResult> Detail(string Id = "")
		{
			if (Id == null) return RedirectToAction("Index");

			var productbyId = _datacontext.Products.Include(p=>p.DanhGias).Where(p => p.Id == Id).FirstOrDefault();
			var namesp = productbyId.Name.ToString();
			ViewBag.Name = namesp;



			if (productbyId == null) return RedirectToAction("Index");

			// Lấy sản phẩm liên quan và kiểm tra điều kiện IsVisible
			var sanphamlienquan = await _datacontext.Products
				.Where(p => p.CategoryId == productbyId.CategoryId
							&& p.Id != productbyId.Id
							&& p.IsVisible == true) // Kiểm tra IsVisible
				.Take(10)
				.ToListAsync();

			ViewBag.SanPhamLienquan = sanphamlienquan;
			var view = new ProductDanhGiaModel
			{
				ProductDetail = productbyId,
				DanhGias = productbyId.DanhGias,


			};
			
			
			

			return View(view);
		}*/
		public async Task<IActionResult> Detail(string Id = "")
		{
			// Nếu không có Id sản phẩm, chuyển hướng về trang danh sách sản phẩm
			if (string.IsNullOrEmpty(Id)) return RedirectToAction("Index");

			// Lấy sản phẩm theo Id
			var productbyId = await _datacontext.Products
				.Include(p => p.DanhGias) // Lấy các đánh giá liên quan
				.FirstOrDefaultAsync(p => p.Id == Id);

			// Nếu sản phẩm không tồn tại, chuyển hướng về trang danh sách sản phẩm
			if (productbyId == null) return RedirectToAction("Index");

			// Lấy tên sản phẩm để hiển thị trên View
			ViewBag.Name = productbyId.Name;

			// Sắp xếp danh sách đánh giá theo thời gian mới nhất trước
			var danhGiasSorted = productbyId.DanhGias
				.OrderByDescending(dg => dg.CreatedAt) // Sắp xếp theo thời gian mới nhất
				.ToList(); // Lấy tất cả đánh giá

			// ViewModel cho sản phẩm và đánh giá
			var viewModel = new ProductDanhGiaModel
			{
				ProductDetail = productbyId,
				DanhGias = danhGiasSorted
			};

			// Lấy sản phẩm liên quan
			var sanphamlienquan = await _datacontext.Products
				.Where(p => p.CategoryId == productbyId.CategoryId
							&& p.Id != productbyId.Id
							&& p.IsVisible) // Kiểm tra điều kiện IsVisible
				.Take(10)
				.ToListAsync();

			// Truyền danh sách sản phẩm liên quan vào ViewBag
			ViewBag.SanPhamLienquan = sanphamlienquan;

			return View(viewModel);
		}


		[HttpPost]
		public async Task<IActionResult> TimKiem(string searchkey)
		{
			var products = await _datacontext.Products.Where(p => p.Name.Contains(searchkey) || p.Description.Contains(searchkey)).ToListAsync();
			ViewBag.Keyword = searchkey;
			return View(products);
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
		public async Task<IActionResult> Showfull(string Id)
		{
			if (string.IsNullOrEmpty(Id)) return RedirectToAction("Index");

			var productbyId = await _datacontext.Products
				.Include(p => p.DanhGias)
				.Where(p => p.Id == Id)
				.FirstOrDefaultAsync();

			if (productbyId == null) return RedirectToAction("Index");

			// Tạo model để gửi dữ liệu qua view
			var viewModel = new ProductDanhGiaModel
			{
				ProductDetail = productbyId,
				DanhGias = productbyId.DanhGias.OrderByDescending(d => d.CreatedAt).ToList()  // Sắp xếp theo ngày tạo
			};

			return View(viewModel);
		}



	}
}
