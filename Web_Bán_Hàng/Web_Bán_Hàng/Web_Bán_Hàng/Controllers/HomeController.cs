using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Controllers
{
    public class HomeController : Controller
    {
        private readonly Datacontext _datacontext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, Datacontext context)
        {

            _logger = logger;
            _datacontext = context;
        }

		public IActionResult Index(int pg = 1)
		{
			// Lấy danh sách sản phẩm kết hợp với thông tin Category và Brand
			List<ProductModel> products = _datacontext.Products
													   .Include(p => p.Category)
													   .Include(p => p.Brand)
													   .ToList();

			const int KichThuoc = 10; // Số sản phẩm mỗi trang
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
    }
}
