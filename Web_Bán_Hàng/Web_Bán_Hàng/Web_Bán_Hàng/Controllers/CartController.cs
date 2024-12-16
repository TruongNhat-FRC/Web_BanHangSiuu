using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;
using Web_Bán_Hàng.Models.ViewModel;

namespace Web_Bán_Hàng.Controllers
{
	public class CartController : Controller
	{
		private readonly Datacontext _datacontext;
		public CartController(Datacontext context) { 
			_datacontext = context;
		
		}
		public IActionResult Index()
		{
			List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            var PhanTramGiamCookie = Request.Cookies["CouponGia"];
            decimal PhanTramhtml = 0;

            if (!string.IsNullOrEmpty(PhanTramGiamCookie))
            {
                PhanTramhtml = decimal.Parse(PhanTramGiamCookie); 
            }



            var PhiShipCookie = Request.Cookies["Phiship"];
            decimal Phishiphtml = 0;
            if (PhiShipCookie != null)
            {
                var shippingPriceJson = PhiShipCookie;
                Phishiphtml = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }


            CartItemViewModel cart = new()
			{
				CartItem = cartItem,
				GrandTotal = cartItem.Sum(p => p.Quantity * p.Price),
                Phiship = Phishiphtml,
                PhanTramGiam = PhanTramhtml
                

			};

			return View(cart);
		}

		public async Task<IActionResult> Add(string id)
		{
			// Kiểm tra nếu người dùng chưa đăng nhập
			if (!User.Identity.IsAuthenticated)
			{
				TempData["error"] = "Bạn cần đăng nhập để bắt đầu mua sắm.";
				return RedirectToAction("DangNhap", "Account");
			}

			ProductModel product = await _datacontext.Products.FindAsync(id);
			if (product == null || product.Quantity <= 0)
			{
				TempData["error"] = "Sản phẩm đã hết hàng.";
				return Redirect(Request.Headers["Referer"].ToString());
			}

			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == id);

			if (cartItem == null)
			{
				if (product.Quantity < 1)
				{
					TempData["error"] = "Không đủ sản phẩm trong kho.";
					return Redirect(Request.Headers["Referer"].ToString());
				}

				cart.Add(new CartItemModel(product));
			}
			else
			{
				if (cartItem.Quantity + 1 > product.Quantity)
				{
					TempData["error"] = "Số lượng yêu cầu vượt quá số lượng có sẵn.";
					return Redirect(Request.Headers["Referer"].ToString());
				}

				cartItem.Quantity += 1;
			}

			HttpContext.Session.SetJson("Cart", cart);
			TempData["success"] = "Đã Thêm Sãn Sàng Thanh Toán";
			return Redirect(Request.Headers["Referer"].ToString());
		}


		public async Task<IActionResult> Decrease(string id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    --cartItem.Quantity;
                }
                else
                {
                    cart.RemoveAll(p => p.ProductId == id);
                }
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["success"] = "Giảm số lượng sản phẩm thành công";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Increase(string id)
        {
            ProductModel product = await _datacontext.Products.FindAsync(id);
            if (product == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                if (cartItem.Quantity + 1 > product.Quantity)
                {
                    TempData["error"] = "Số lượng yêu cầu vượt quá số lượng có sẵn.";
                    return RedirectToAction("Index");
                }

                ++cartItem.Quantity;
            }

            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Tăng số lượng sản phẩm thành công";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(string id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			cart.RemoveAll(p => p.ProductId == id);
			if(cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);

			}
			TempData["success"] = "Đã xóa sản phẩm";

			return RedirectToAction("Index");

		}
		public async Task<IActionResult> Clear(string id)
		{
			HttpContext.Session.Remove("Cart");
			TempData["success"] = "Đã xóa toàn bộ giỏ hàng";

			return RedirectToAction("Index");


		}

		[HttpPost]
        public async Task<IActionResult> TinhPhiShip(VanChuyenModel vanchuyen, string quan,string tinh, string phuong)
        {
            var Diachicotontai = await _datacontext.VanChuyens.FirstOrDefaultAsync(x => x.thanhpho == tinh && x.Quan == quan && x.Phuong_xa == phuong);


            decimal phiship = 0; // Set mặc định giá tiền

            if (Diachicotontai != null)
            {
                phiship = Diachicotontai.Gia;
            }
            else
            {
                // Set mặc định giá tiền nếu không tìm thấy
                phiship = 30000;
            }

            var shippingPriceJson = JsonConvert.SerializeObject(phiship);

            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true // using HTTPS
                };

                // Thêm giá trị vào cookie
                Response.Cookies.Append("Phiship", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding shipping price cookie: {ex.Message}");
            }

            return Json(new { phiship });
        }
        [HttpPost]
        public IActionResult DeletePhiShip()
        {
            Response.Cookies.Delete("Phiship");
            return RedirectToAction("Index", "Cart");
        }
        [HttpPost]
        public async Task<IActionResult> GetCoupon(string coupon_value)
        {
            // Tìm coupon hợp lệ trong bảng KhuyenMais
            var validCoupon = await _datacontext.KhuyenMais
                .FirstOrDefaultAsync(x => x.Name == coupon_value && x.SoLuong > 0 && x.trangthai == 1); // Kiểm tra trạng thái coupon (trangthai == 1 là còn hiệu lực)

            if (validCoupon != null)
            {
                // Nếu coupon hợp lệ, lấy giá trị Gia
                decimal couponGia = validCoupon.Gia;

                // Thiết lập cookie với giá trị giảm giá
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30), // Cookie hết hạn sau 30 phút
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                // Lưu giá trị Gia vào cookie dưới dạng string
                Response.Cookies.Append("CouponGia", couponGia.ToString(), cookieOptions);

                // Trả về kết quả thành công
                return Json(new { success = true, message = "Mã giảm giá đã được áp dụng thành công", CouponGia = couponGia });
            }
            else
            {
                // Nếu không tìm thấy coupon hoặc không hợp lệ
                return Json(new { success = false, message = "Mã giảm giá không tồn tại hoặc đã hết hạn" });
            }
        }









    }

}
