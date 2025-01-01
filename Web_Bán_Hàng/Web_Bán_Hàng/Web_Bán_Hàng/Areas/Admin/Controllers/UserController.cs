using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Hiển thị danh sách người dùng
        /*public IActionResult Index()
		{
            var users = _userManager.Users.Where(u => !u.IsDeleted).ToList();
            return View(users);
        }*/
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users
                .Where(u => !u.IsDeleted)
                .ToList();

            foreach (var user in users)
            {
                // Lấy danh sách quyền cho từng user
                var roles = await _userManager.GetRolesAsync(user);
                user.Roles = roles.ToList(); // Gắn danh sách quyền vào thuộc tính Roles
            }

            return View(users);
        }



        // Hiển thị trang chỉnh sửa quyền
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                AvailableRoles = roles.Select(role => new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = userRoles.Contains(role.Name)
                }).ToList()
            };

            return View(model);
        }

        // Cập nhật quyền cho người dùng
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.AvailableRoles
                                      .Where(role => role.IsSelected)
                                      .Select(role => role.RoleName)
                                      .ToList();

            var rolesToAdd = selectedRoles.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(selectedRoles);

            await _userManager.AddToRolesAsync(user, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            TempData["Success"] = "Cập nhật quyền thành công.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> EditInfo(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new ThongTinNguoiDungViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditInfo(ThongTinNguoiDungViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật thông tin cá nhân thành công.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Pass user details to the view
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Mark the user as deleted
            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Người dùng đã được đánh dấu là đã xóa.";
                return RedirectToAction("Index"); // Redirect to the user list
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            TempData["Error"] = "Đã xảy ra lỗi khi đánh dấu người dùng là đã xóa.";
            return RedirectToAction("Index"); // Return to the user list even on failure
        }
        [HttpGet]
        public async Task<IActionResult> DanhSachNguoiDungBiXoa()
        {
            var users = await _userManager.Users
                .Where(u => u.IsDeleted)
                .ToListAsync();
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> PhucHoiNguoiDung(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            if (user.IsDeleted)
            {
                user.IsDeleted = false;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Phục hồi người dùng thành công.";
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi phục hồi người dùng.";
                }
            }
            else
            {
                TempData["Error"] = "Người dùng này không bị xóa.";
            }

            return RedirectToAction("DanhSachNguoiDungBiXoa");
        }




    }
}
