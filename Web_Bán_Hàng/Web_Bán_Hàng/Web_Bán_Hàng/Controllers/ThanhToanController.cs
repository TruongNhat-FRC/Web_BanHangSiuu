using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Controllers
{
	

	public class ThanhToanController : Controller
	{
		private readonly Datacontext _datacontext;
        private readonly IEmailSender _emailSender;
		public ThanhToanController(Datacontext context, IEmailSender emailSender)
		{
			_datacontext = context;
            _emailSender = emailSender;

		}
		public IActionResult Index()
		{
			return View();
		}
        public async Task<IActionResult> ThanhToan()
        {
            var usermail = User.FindFirstValue(ClaimTypes.Email);
            var fullName = User.FindFirstValue(ClaimTypes.Name); // Lấy tên đầy đủ từ Claim
            var user = await _datacontext.Users.FirstOrDefaultAsync(u => u.Email == usermail);
            var fullName1 = user?.FullName ?? "Người dùng"; // Lấy tên đầy đủ từ cơ sở dữ liệu, nếu không có thì hiển thị "Người dùng"


            if (usermail == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var PhiShipCookie = Request.Cookies["Phiship"];
                decimal Phishiphtml = 0;

                if (PhiShipCookie != null)
                {
                    var shippingPriceJson = PhiShipCookie;
                    Phishiphtml = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
                }
                var madonhang = Guid.NewGuid().ToString();
                var orderItem = new DonHangModel();
                orderItem.MaDonHang = madonhang;
                orderItem.PhiShip = Phishiphtml;
                orderItem.MaNguoiDung = usermail;
                orderItem.TrangThai = 1; // Trạng thái là "Đã thanh toán"
                orderItem.NgayDat = DateTime.Now;

                // Tính tổng tiền cho đơn hàng
                List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                decimal totalAmount = 0;
				var orderDetailsHtml = "<h3>Chi tiết đơn hàng của bạn</h3><table border='1'><tr><th>Sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr>";
				foreach (var item in cartItem)
                {
                    var chitietdonhang = new ChiTietDonHang();
                    chitietdonhang.MaDonHang = madonhang;
                    chitietdonhang.MaNguoiDung = usermail;
                    chitietdonhang.Gia1mon = item.Price;
                    chitietdonhang.Soluong = item.Quantity;
                    chitietdonhang.MaSanPham = item.ProductId;
                    chitietdonhang.ThanhTien = chitietdonhang.Gia1mon * chitietdonhang.Soluong;

                    totalAmount += chitietdonhang.ThanhTien; // Cộng dồn tổng tiền

                    _datacontext.Add(chitietdonhang); // Thêm chi tiết đơn hàng

                    // Cập nhật số lượng sản phẩm sau khi thanh toán
                    var product = await _datacontext.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity; // Giảm số lượng sản phẩm đi
                        _datacontext.Update(product); // Cập nhật sản phẩm
                    }
					orderDetailsHtml += $"<tr><td>{item.ProductName}</td><td>{item.Quantity}</td><td>{item.Price.ToString("#,0")} VND</td><td>{(item.Price * item.Quantity).ToString("#,0")} VND</td></tr>";
				}
				orderDetailsHtml += $"</table><br><strong>Tổng tiền: {totalAmount.ToString("#,0")} VND</strong><br><strong>Nếu có gì sai sót bạn có thể liên hệ theo số đường dây nóng sau để được hỗ trợ sớm nhất : 0377875295 <strong>";

				// Cập nhật tổng tiền cho đơn hàng
				orderItem.TongTienCuoi = totalAmount;

                // Lưu đơn hàng và chi tiết đơn hàng
                _datacontext.Add(orderItem);
                await _datacontext.SaveChangesAsync(); // Lưu tất cả thay đổi

                // Xóa giỏ hàng sau khi thanh toán
                HttpContext.Session.Remove("Cart");
                Response.Cookies.Delete("Phiship");
                //Send mail order when success
                var receiver = usermail;
				var subject = "Trạng thái đặt hàng hàng .";
				var message = $"Xin chào {fullName1} <br>Đơn hàng của bạn đã được tạo thành công. Cảm ơn bạn đã mua sắm tại chúng tôi.<br>{orderDetailsHtml}";
				try
				{
					// Gửi email
					await _emailSender.SendEmailAsync(receiver, subject, message);
					TempData["success"] = "Đã tạo đơn hàng thành công, vui lòng kiểm tra email để biết thêm chi tiết.";
				}
				catch (Exception ex)
				{
					// Log lỗi nếu có
					TempData["error"] = "Có lỗi xảy ra khi gửi email: " + ex.Message;
				}
				return RedirectToAction("Index", "Cart");
            }
            return View();
        }


    }
}
