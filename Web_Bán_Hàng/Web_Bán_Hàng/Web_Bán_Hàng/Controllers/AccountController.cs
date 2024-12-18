using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;
using Web_Bán_Hàng.Models.ViewModel;

namespace Web_Bán_Hàng.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly Datacontext _datacontext;
        private readonly IEmailSender _emailSender;
        public AccountController( UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, Datacontext datacontext, IEmailSender emailSender)
		{
			_signInManager = signInManager;
			_userManager = userManager;
            _datacontext = datacontext;
            _emailSender = emailSender;
		}
		public IActionResult Index()
		{
			return View();
		}
        public List<CartHistoryModel> GetCartHistoryForUser(string userId)
        {
            return _datacontext.LichSuGioHang
                .Where(c => c.UserId == userId && !c.IsCheckedOut) // Lọc giỏ hàng chưa thanh toán
                .ToList();
        }

        public IActionResult DangNhap(string returnURL)
		{
			return View(new DangNhapViewModel { ReturnUrl = returnURL });
		}
        [HttpPost]
        public async Task<IActionResult> GuiMailXacNhanMatKhau(AppUserModel user)
        {
            // Kiểm tra email có tồn tại trong hệ thống hay không
            var checkMail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (checkMail == null)
            {
                TempData["error"] = "Email not found";
                return RedirectToAction("QuenMatKhau", "Account");
            }
            else
            {
                // Tạo mã token mới
                string token = Guid.NewGuid().ToString();

                // Cập nhật token vào tài khoản người dùng
                checkMail.Token = token;
                _datacontext.Update(checkMail);
                await _datacontext.SaveChangesAsync();

                // Gửi email reset mật khẩu
                var receiver = checkMail.Email;
                var subject = $"Change password for user {checkMail.Email}";
                var message = "Click on the link to change your password: " + $"<a href=\"{Request.Scheme}://{Request.Host}/Account/MatKhauMoi?email={checkMail.Email}&token={token}\">Cập Nhật PassWord</a>";


                await _emailSender.SendEmailAsync(receiver, subject, message);

                TempData["success"] = "Một email đã được gửi đến địa chỉ email đã đăng ký của bạn kèm theo hướng dẫn đặt lại mật khẩu.";
                return RedirectToAction("QuenMatKhau", "Account");
            }
        }




        public async Task<IActionResult> CapNhatMatKhauMoi(AppUserModel user, string token)
        {
            var checkUser = await _userManager.Users
                .Where(u => u.Email == user.Email)
                .Where(u => u.Token == token)
                .FirstOrDefaultAsync();

            if (checkUser != null)
            {
                // Update user with new password and token
                string newToken = Guid.NewGuid().ToString();

                // Hash the new password
                var passwordHasher = new PasswordHasher<AppUserModel>();
                var passwordHash = passwordHasher.HashPassword(checkUser, user.PasswordHash);

                checkUser.PasswordHash = passwordHash;
                checkUser.Token = newToken;

                await _userManager.UpdateAsync(checkUser);

                TempData["success"] = "Cập Nhật Mật Khẩu Thành Công.";
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                TempData["error"] = "Không tìm thấy emil.";
                return RedirectToAction("QuenMatKhau", "Account");
            }

            return View();
        }


        public async Task<IActionResult> QuenMatKhau(string returnURL)
        {
            return View();
        }
        public async Task<IActionResult> MatKhauMoi(AppUserModel user, string token)
        {
            var checkUser = await _userManager.Users
                .Where(u => u.Email == user.Email)
                .Where(u => u.Token == token)
                .FirstOrDefaultAsync();

            if (checkUser != null)
            {
                ViewBag.Email = checkUser.Email;
                ViewBag.Token = token;
            }
            else
            {
                TempData["error"] = "Email tìm không thấy hoặc không đúng";
                return RedirectToAction("QuenMatKhau", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapViewModel dangnhap)
        {
            HttpContext.Session.Remove("Cart");
            if (ModelState.IsValid)
            {
                // Xác thực người dùng với tên người dùng và mật khẩu
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(dangnhap.UserName, dangnhap.Password, false, false);

                if (result.Succeeded)
                {
                    // Lấy thông tin người dùng từ UserManager
                    var user = await _userManager.FindByNameAsync(dangnhap.UserName);

                    // Kiểm tra xem người dùng có bị xóa hay không
                    if (user != null && user.IsDeleted)
                    {
                        // Nếu người dùng bị xóa, không cho phép đăng nhập
                        ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa.");
                        return View();
                    }
                    // Lấy lịch sử giỏ hàng của người dùng
                    var cartHistoryItems = GetCartHistoryForUser(user.Id); // Lấy giỏ hàng từ lịch sử giỏ hàng

                    /*if (cartHistoryItems.Any())
                    {
                        // Phục hồi giỏ hàng từ lịch sử
                        var cartItems = cartHistoryItems.Select(item => new CartItemModel
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Image = item.Image
                        }).ToList();

                        // Lưu giỏ hàng vào session
                        HttpContext.Session.SetJson("Cart", cartItems);
                    }*/

                    // Nếu tất cả đều ok, chuyển hướng người dùng
                    return Redirect(dangnhap.ReturnUrl ?? "/");
                }

                // Nếu đăng nhập thất bại
                ModelState.AddModelError("", "Tên người dùng hoặc mật khẩu sai");
            }

            return View();
        }

        public IActionResult DangKy()
		{
			
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> DangKy(UserModel nguoidung)
        {
            if (ModelState.IsValid)
            {
                // Tạo người dùng mới
                AppUserModel nguoi_dung_moi = new AppUserModel
                {
                    UserName = nguoidung.UserName,
                    Email = nguoidung.Email,
                    PhoneNumber = nguoidung.PhoneNumber,
                    FullName = nguoidung.FullName 
                };
                IdentityResult result = await _userManager.CreateAsync(nguoi_dung_moi, nguoidung.Password);

                if (result.Succeeded)
                {
                    // Gán quyền mặc định "NguoiDung" cho tài khoản mới
                    var roleResult = await _userManager.AddToRoleAsync(nguoi_dung_moi, "NguoiDung");
                    if (roleResult.Succeeded)
                    {
                        TempData["success"] = "Đã tạo tài khoản thành công với quyền NguoiDung.";
                        return RedirectToAction("DangNhap", "Account");
                    }

                    // Xử lý lỗi nếu không gán được quyền
                    foreach (IdentityError error in roleResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                // Xử lý lỗi tạo tài khoản
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(nguoidung);
        }











        /*public async Task<IActionResult> DangXuat(string returnURL = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnURL);

		}*/



        ///Lấy giỏ hàng
        public List<CartItemModel> GetCartItemsFromSession()
        {
            return HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        }



        /*public async Task<IActionResult> DangXuat()
        {
            var userId = _userManager.GetUserId(User); // Lấy UserId của người dùng đang đăng nhập

            // Kiểm tra xem người dùng có giỏ hàng chưa thanh toán không
            var cartItems = GetCartItemsFromSession(); // Lấy giỏ hàng của người dùng

            // Lưu giỏ hàng vào lịch sử giỏ hàng
            foreach (var item in cartItems)
            {
                var cartHistoryItem = new CartHistoryModel
                {
                    UserId = userId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Image = item.Image,
                    AddedDate = DateTime.Now,
                    IsCheckedOut = false // Chưa thanh toán
                };

                _datacontext.LichSuGioHang.Add(cartHistoryItem);

            }

            await _datacontext.SaveChangesAsync();

            // Đăng xuất người dùng
            await _signInManager.SignOutAsync();
            TempData["success"] = "Đã lưu những sản phẩm chưa thanh toán vào giỏ hàng";

            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ hoặc trang đăng nhập
        }*/
        public async Task<IActionResult> DangXuat()
        {
            var userId = _userManager.GetUserId(User); // Lấy UserId của người dùng đang đăng nhập

            // Lấy giỏ hàng của người dùng
            var cartItems = GetCartItemsFromSession();

            // Kiểm tra xem giỏ hàng có sản phẩm hay không
            if (cartItems.Any())
            {
                // Lưu giỏ hàng vào lịch sử giỏ hàng
                foreach (var item in cartItems)
                {
                    var cartHistoryItem = new CartHistoryModel
                    {
                        UserId = userId,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Image = item.Image,
                        AddedDate = DateTime.Now,
                        IsCheckedOut = false // Chưa thanh toán
                    };

                    _datacontext.LichSuGioHang.Add(cartHistoryItem);
                }

                await _datacontext.SaveChangesAsync();

                // Hiển thị thông báo nếu giỏ hàng có sản phẩm
                TempData["success"] = "Đã lưu những sản phẩm chưa thanh toán vào giỏ hàng";
            }
           

            // Đăng xuất người dùng
            await _signInManager.SignOutAsync();

            // Chuyển hướng về trang chủ hoặc trang đăng nhập
            return RedirectToAction("Index", "Home");
        }



















































        public IActionResult Not_Quyen()
		{
			return View();
		}
        // Hiển thị form cập nhật thông tin cá nhân
        /*public async Task<IActionResult> CapNhatThongTinCaNhan()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("DangNhap");
            }

            var viewModel = new UserModel
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName // Lấy tên đầy đủ

            };

            return View(viewModel);
        }
*/
        // Xử lý logic cập nhật thông tin cá nhân
        /*[HttpPost]
        public async Task<IActionResult> CapNhatThongTinCaNhan(UserModel model)
        {
            // Lấy thông tin người dùng đang đăng nhập
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Người dùng không tồn tại hoặc phiên đăng nhập đã hết hạn.";
                return RedirectToAction("DangNhap");
            }

            // Kiểm tra nếu không có thay đổi nào
            if (user.Email == model.Email && user.PhoneNumber == model.PhoneNumber && user.FullName == model.FullName)
            {
                TempData["info"] = "Không có thay đổi nào để cập nhật.";
                return RedirectToAction("Index", "Home");
            }

            // Cập nhật thông tin
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.FullName = model.FullName;

            // Lưu thay đổi
            var result = await _userManager.UpdateAsync(user);

            // Kiểm tra kết quả cập nhật
            if (result.Succeeded)
            {
                TempData["success"] = "Cập nhật thông tin cá nhân thành công.";
                return RedirectToAction("Index", "Home");
            }

            // Nếu có lỗi, thêm lỗi vào ModelState để hiển thị trên giao diện
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }


            // Nếu có lỗi, trả lại form cùng với thông báo
            return View(model);
        }*/

        public async Task<IActionResult> CapNhatThongTinCaNhan()
        {
            // Lấy userId từ claim của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Lấy thông tin user từ cơ sở dữ liệu dựa trên userId
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Account/CapNhatThongTinCaNhan
        [HttpPost]
        public async Task<IActionResult> CapNhatThongTinCaNhan(AppUserModel user, string OldPassword)
        {
            // Lấy userId từ claim của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userById = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (userById == null)
            {
                return NotFound();
            }

            // Kiểm tra mật khẩu cũ
            var passwordHasher = new PasswordHasher<AppUserModel>();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(userById, userById.PasswordHash, OldPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu cũ không chính xác.");
                return View(user);
            }

            // Hash mật khẩu mới
            var passwordHash = passwordHasher.HashPassword(userById, user.PasswordHash);
            userById.PasswordHash = passwordHash;

            // Cập nhật thông tin user
            userById.UserName = user.UserName;
            userById.Email = user.Email;
            userById.PhoneNumber = user.PhoneNumber;
            userById.FullName = user.FullName;

            // Cập nhật thông tin user vào cơ sở dữ liệu
            _datacontext.Update(userById);
            await _datacontext.SaveChangesAsync();

            TempData["success"] = "Cập nhật thông tin cá nhân thành công";
            return RedirectToAction("CapNhatThongTinCaNhan", "Account");
        }



        public async Task<IActionResult> LichSuGioHang()
        {
            var userId = _userManager.GetUserId(User); // Nếu dùng UserManager để lấy UserId
            var cartHistoryItems = await _datacontext.LichSuGioHang.Where(c => c.UserId == userId).ToListAsync();

            return View(cartHistoryItems);
        }
        [HttpPost]
        public async Task<IActionResult> AddAndRemoveFromHistory(string id, int quantity)
        {
            var userId = _userManager.GetUserId(User); // Lấy UserId của người dùng
            var product = await _datacontext.Products.FindAsync(id); // Lấy thông tin sản phẩm từ cơ sở dữ liệu

            if (product != null)
            {
                // Thêm sản phẩm vào giỏ hàng
                Add(id); // Gọi phương thức Add với id sản phẩm

                // Xóa sản phẩm khỏi lịch sử giỏ hàng
                var cartHistoryItem = await _datacontext.LichSuGioHang
                                                         .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == id && !c.IsCheckedOut);

                if (cartHistoryItem != null)
                {
                    _datacontext.LichSuGioHang.Remove(cartHistoryItem); // Xóa sản phẩm khỏi lịch sử giỏ hàng
                    await _datacontext.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                }
            }

            return RedirectToAction("LichSuGioHang"); // Chuyển hướng về trang lịch sử giỏ hàng
        }
		[HttpPost]
		public async Task<IActionResult> XoaSanPhamTuLichSuGioHang(string productId)
		{
			var userId = _userManager.GetUserId(User); // Lấy UserId của người dùng

			// Lấy sản phẩm từ cơ sở dữ liệu
			var cartHistoryItem = await _datacontext.LichSuGioHang
													 .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId && !c.IsCheckedOut);

			if (cartHistoryItem != null)
			{
				_datacontext.LichSuGioHang.Remove(cartHistoryItem); // Xóa sản phẩm khỏi lịch sử giỏ hàng
				await _datacontext.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
			}

			return RedirectToAction("LichSuGioHang"); // Chuyển hướng về trang lịch sử giỏ hàng
		}



		///Bonus
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
            TempData["success"] = "Thêm vào giỏ hàng thành công";
            return Redirect(Request.Headers["Referer"].ToString());
        }


        public async Task<IActionResult> LichSuMuaHang()
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var donhang = await _datacontext.DonHangs.Where(d => d.MaNguoiDung == userEmail).OrderByDescending(w => w.ID).ToListAsync();

            return View(donhang);
        }
        public async Task<IActionResult> Details(string madonhang)
        {
            var donHang = await _datacontext.DonHangs
                .Include(d => d.ChiTietDonHangs) // Bao gồm các chi tiết đơn hàng
                .ThenInclude(ct => ct.Product) // Bao gồm thông tin sản phẩm (nếu có)
                .FirstOrDefaultAsync(d => d.MaDonHang == madonhang);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
        public async Task<IActionResult> HuyDonHang(string madonhang)
        {
            // Kiểm tra nếu người dùng chưa đăng nhập
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                // Người dùng chưa đăng nhập, chuyển hướng tới trang đăng nhập
                return RedirectToAction("DangNhap", "Account");
            }

            try
            {
                // Tìm đơn hàng dựa trên mã đơn hàng
                var order = await _datacontext.DonHangs
                    .Where(o => o.MaDonHang == madonhang)
                    .FirstAsync();

                // Cập nhật trạng thái của đơn hàng
                order.TrangThai = 3; // 3: trạng thái đã hủy
                _datacontext.Update(order);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _datacontext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi lỗi
                return BadRequest("Lỗi");
            }

            // Chuyển hướng về trang lịch sử đơn hàng
            return RedirectToAction("LichSuMuaHang", "Account");
        }





    }
}
