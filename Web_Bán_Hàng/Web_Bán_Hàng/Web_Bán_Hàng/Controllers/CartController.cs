using Microsoft.AspNetCore.Mvc;
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
			CartItemViewModel cart = new()
			{
				CartItem = cartItem,
				GrandTotal = cartItem.Sum(p => p.Quantity * p.Price)

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



	}

}
