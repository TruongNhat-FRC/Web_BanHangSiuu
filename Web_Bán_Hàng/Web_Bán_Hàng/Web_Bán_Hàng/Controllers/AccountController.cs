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
                .Where(c => c.UserId == userId && !c.IsCheckedOut) 
                .ToList();
        }

        public IActionResult DangNhap(string returnURL)
		{
			return View(new DangNhapViewModel { ReturnUrl = returnURL });
		}
        [HttpPost]
        public async Task<IActionResult> GuiMailXacNhanMatKhau(AppUserModel user)
        {
            var checkMail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (checkMail == null)
            {
                TempData["error"] = "Không tìm thấy gmail";
                return RedirectToAction("QuenMatKhau", "Account");
            }
            else
            {
                string token = Guid.NewGuid().ToString();

                checkMail.Token = token;
                _datacontext.Update(checkMail);
                await _datacontext.SaveChangesAsync();

                var receiver = checkMail.Email;
                var subject = $"Thay đổi mật khẩu cho địa chỉ: {checkMail.Email}";
                var message = "Nhấp vào link này để tiến hành đổi mật khẩu: " + $"<a href=\"{Request.Scheme}://{Request.Host}/Account/MatKhauMoi?email={checkMail.Email}&token={token}\">Cập Nhật PassWord</a>";


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
                string newToken = Guid.NewGuid().ToString();

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
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(dangnhap.UserName, dangnhap.Password, false, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(dangnhap.UserName);

                    if (user != null && user.IsDeleted)
                    {
                        ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa.");
                        return View();
                    }
                    var cartHistoryItems = GetCartHistoryForUser(user.Id); 

                    
                    return Redirect(dangnhap.ReturnUrl ?? "/");
                }

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
				var existingUser = await _userManager.FindByEmailAsync(nguoidung.Email);
				if (existingUser != null)
				{
					ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng chọn email khác.");
					return View(nguoidung);
				}
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
                    var roleResult = await _userManager.AddToRoleAsync(nguoi_dung_moi, "NguoiDung");
                    if (roleResult.Succeeded)
                    {
                        TempData["success"] = "Đã tạo tài khoản thành công.";
                        return RedirectToAction("DangNhap", "Account");
                    }

                    foreach (IdentityError error in roleResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(nguoidung);
        }





        
        public List<CartItemModel> GetCartItemsFromSession()
        {
            return HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        }



        
        public async Task<IActionResult> DangXuat()
        {
            var userId = _userManager.GetUserId(User); 

            
            var cartItems = GetCartItemsFromSession();

           
            if (cartItems.Any())
            {
                
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

                
                TempData["success"] = "Đã lưu những sản phẩm chưa thanh toán vào giỏ hàng";
            }
           

            
            await _signInManager.SignOutAsync();

            
            return RedirectToAction("Index", "Home");
        }



        public IActionResult Not_Quyen()
		{
			return View();
		}
        
        

        public async Task<IActionResult> CapNhatThongTinCaNhan()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatThongTinCaNhan(AppUserModel user, string OldPassword)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userById = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (userById == null)
            {
                return NotFound();
            }

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

            userById.UserName = user.UserName;
            userById.Email = user.Email;
            userById.PhoneNumber = user.PhoneNumber;
            userById.FullName = user.FullName;

            _datacontext.Update(userById);
            await _datacontext.SaveChangesAsync();

            TempData["success"] = "Cập nhật thông tin cá nhân thành công";
            return RedirectToAction("CapNhatThongTinCaNhan", "Account");
        }



        public async Task<IActionResult> LichSuGioHang()
        {
            var userId = _userManager.GetUserId(User); 
            var cartHistoryItems = await _datacontext.LichSuGioHang.Where(c => c.UserId == userId).ToListAsync();

            return View(cartHistoryItems);
        }
        [HttpPost]
        public async Task<IActionResult> AddAndRemoveFromHistory(string id, int quantity)
        {
            var userId = _userManager.GetUserId(User); 
            var product = await _datacontext.Products.FindAsync(id); 

            if (product != null)
            {
                
                Add(id); 

                
                var cartHistoryItem = await _datacontext.LichSuGioHang
                                                         .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == id && !c.IsCheckedOut);

                if (cartHistoryItem != null)
                {
                    _datacontext.LichSuGioHang.Remove(cartHistoryItem); 
                    await _datacontext.SaveChangesAsync(); 
                }
            }

            return RedirectToAction("LichSuGioHang"); 
        }
		[HttpPost]
		public async Task<IActionResult> XoaSanPhamTuLichSuGioHang(string productId)
		{
			var userId = _userManager.GetUserId(User); 

			var cartHistoryItem = await _datacontext.LichSuGioHang
													 .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId && !c.IsCheckedOut);

			if (cartHistoryItem != null)
			{
				_datacontext.LichSuGioHang.Remove(cartHistoryItem); 
				await _datacontext.SaveChangesAsync(); 
			}

			return RedirectToAction("LichSuGioHang"); 
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
                .Include(d => d.ChiTietDonHangs) 
                .ThenInclude(ct => ct.Product) 
                .FirstOrDefaultAsync(d => d.MaDonHang == madonhang);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
        public async Task<IActionResult> HuyDonHang(string madonhang)
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToAction("DangNhap", "Account");
            }

            try
            {
                var order = await _datacontext.DonHangs
                    .Where(o => o.MaDonHang == madonhang)
                    .FirstAsync();

                order.TrangThai = 3; // 3: trạng thái đã hủy
                _datacontext.Update(order);

                await _datacontext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi");
            }

            return RedirectToAction("LichSuMuaHang", "Account");
        }





    }
}
