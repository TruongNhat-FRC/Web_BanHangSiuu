using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;
using Web_Bán_Hàng.Models.ViewModel;

namespace Web_Bán_Hàng.Controllers
{
    public class HomeController : Controller
    {
        private readonly Datacontext _datacontext;
        private readonly ILogger<HomeController> _logger;
		private readonly UserManager<AppUserModel> _userManager;

		public HomeController(ILogger<HomeController> logger, Datacontext context, UserManager<AppUserModel> userManager)
        {

            _logger = logger;
            _datacontext = context;
            _userManager = userManager;
        }

        public IActionResult Index(int pg = 1)
        {
            // Lấy danh sách sản phẩm kết hợp với thông tin Category và Brand
            List<ProductModel> products = _datacontext.Products
                                                       .Include(p => p.Category)
                                                       .Include(p => p.Brand)
                                                       .ToList();
            var slide = _datacontext.Sliders.Where(s => s.Status == 1).ToList();
            ViewBag.Slide = slide;
			
			var sanPhamHot = _datacontext.Products
										  .Where(p => p.IsVisible)  
										  .OrderByDescending(p => p.PurchaseCount) 
										  .Take(10)  
										  .ToList();

			
			ViewBag.SanPhamHot = sanPhamHot;

			const int KichThuoc = 8; // Số sản phẩm mỗi trang
            if (pg < 1)
            {
                pg = 1;
            }

            // Tính tổng số sản phẩm
            int count = products.Count();

            // Tạo đối tượng phân trang
            var trang = new PhanTrang(count, pg, KichThuoc);

            // Tính số sản phẩm cần bỏ qua
            int buocnhay = (pg - 1) * KichThuoc;

            // Lấy sản phẩm cho trang hiện tại
            var data = products.Skip(buocnhay).Take(trang.KichThuocTrang).ToList();

            // Gửi thông tin phân trang vào ViewBag
            ViewBag.Trang = trang;

            // Trả về danh sách sản phẩm cho View
            return View(data);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("KhongTimThay");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            }
        }
        public IActionResult Contact()
        {
            return View();
        }
        public async Task<IActionResult> LienHe()
        {
            var lienhe = await _datacontext.LienHes.FirstAsync();
            return View(lienhe);
        }
		public async Task<IActionResult> ThemYeuThich(string Id, YeuThichModel yeuthich)
		{
			// Lấy thông tin người dùng hiện tại
			var user = await _userManager.GetUserAsync(User);
			

			// Kiểm tra xem sản phẩm đã có trong danh sách yêu thích chưa
			var sptontai = await _datacontext.YeuThichs
				.FirstOrDefaultAsync(x => x.ProductId == Id && x.UserId == user.Id );

			if (sptontai != null)
			{
				return BadRequest(new { success = false, message = "Sản phẩm đã tồn tại trong danh sách yêu thích." });
			}

			// Nếu chưa tồn tại, thêm sản phẩm vào danh sách yêu thích
			yeuthich.ProductId = Id;
			yeuthich.UserId = user.Id;
			_datacontext.Add(yeuthich);

			try
			{
				await _datacontext.SaveChangesAsync();
				return Ok(new { success = true, message = "Thêm vào yêu thích thành công." });
			}
			catch (Exception)
			{
				return StatusCode(500, new { success = false, message = "Có lỗi trong quá trình thêm." });
			}
		}

		public async Task<IActionResult> ThemSoSanh(string Id, SoSanhModel sosanh)
		{
			var user = await _userManager.GetUserAsync(User);
			// Kiểm tra xem sản phẩm đã có trong danh sách yêu thích chưa
			var sptontai = await _datacontext.SoSanhs
				.FirstOrDefaultAsync(x => x.ProductId == Id && x.UserId == user.Id);

			if (sptontai != null)
			{
				return BadRequest(new { success = false, message = "Sản phẩm đã tồn tại trong danh sách yêu thích." });
			}
			sosanh.ProductId = Id;
			sosanh.UserId = user.Id;
			_datacontext.Add(sosanh);
			try
			{
				await _datacontext.SaveChangesAsync();
				return Ok(new { success = true, message = "Thêm vào để so sánh thành công " });
			}
			catch (Exception)
			{
				return StatusCode(500, "Có lỗi trong quá trình thêm");
			}
		}

        public async Task<IActionResult> ListYeuThich()
        {
            // Lấy Id người dùng hiện tại
            var userId = _userManager.GetUserId(User);

            // Truy vấn danh sách yêu thích của người dùng hiện tại
            var List_yeuthich = await (from y in _datacontext.YeuThichs
                                       join p in _datacontext.Products on y.ProductId equals p.Id
                                       join u in _datacontext.Users on y.UserId equals u.Id
                                       where y.UserId == userId // Lọc theo UserId hiện tại
                                       select new YeuThichViewModel
                                       {
                                           Id = y.Id,
                                           ProductId = p.Id,
                                           ProductName = p.Name,
                                           Description = p.Description,
                                           Price = p.Price,
                                           Image = p.Image,
                                           UserName = u.UserName,
                                           UserId = u.Id,
                                           TrangThai = p.IsVisible
                                       }).ToListAsync();

            return View(List_yeuthich);
        }



        public async Task<IActionResult> ListSoSanh()
        {
            // Lấy Id người dùng hiện tại
            var userId = _userManager.GetUserId(User);

            // Lấy danh sách sản phẩm so sánh của người dùng hiện tại
            var List_sosanh = await (from s in _datacontext.SoSanhs
                                     join p in _datacontext.Products on s.ProductId equals p.Id
                                     join u in _datacontext.Users on s.UserId equals u.Id
                                     where s.UserId == userId // Lọc theo UserId hiện tại
                                     select new SoSanhViewModel
                                     {
                                         Id = s.Id,
                                         ProductId = p.Id,
                                         ProductName = p.Name,
                                         Description = p.Description,
                                         Price = p.Price,
                                         Image = p.Image,
                                         UserName = u.UserName,
                                         UserId = u.Id,
                                         TrangThai = p.IsVisible
                                     }).ToListAsync();

            return View(List_sosanh);
        }

        public async Task<IActionResult> DeleteYeuThich(int Id)
        {
            // Tìm đối tượng yêu thích theo Id
            var yeuthich = await _datacontext.YeuThichs.FindAsync(Id);

            if (yeuthich != null) // Kiểm tra xem đối tượng có tồn tại không
            {
                // Xóa đối tượng khỏi DbContext
                _datacontext.YeuThichs.Remove(yeuthich);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _datacontext.SaveChangesAsync();

                // Thông báo thành công
                TempData["success"] = "Yêu Thích đã được xóa thành công";
            }
            else
            {
                // Thông báo khi đối tượng không tồn tại
                TempData["error"] = "Không tìm thấy sản phẩm trong danh sách yêu thichs";
            }

            // Chuyển hướng về trang Wishlist
            return RedirectToAction("ListYeuThich","Home");
        }
        public async Task<IActionResult> DeleteSoSanh(int Id)
        {
            // Tìm đối tượng yêu thích theo Id
            var sosanh = await _datacontext.SoSanhs.FindAsync(Id);

            if (sosanh != null) // Kiểm tra xem đối tượng có tồn tại không
            {
                // Xóa đối tượng khỏi DbContext
                _datacontext.SoSanhs.Remove(sosanh);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _datacontext.SaveChangesAsync();

                // Thông báo thành công
                TempData["success"] = "So sánh đã được xóa thành công";
            }
            else
            {
                // Thông báo khi đối tượng không tồn tại
                TempData["error"] = "Không tìm thấy sản phẩm trong danh sách so sánh";
            }

            // Chuyển hướng về trang Wishlist
            return RedirectToAction("ListSoSanh", "Home");
        }
		




	}
}
