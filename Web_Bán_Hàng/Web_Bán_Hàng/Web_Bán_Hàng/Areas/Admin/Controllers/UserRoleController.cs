using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserRoleController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public UserRoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUserModel> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        //danh sách quyền
        public IActionResult Index()
        {
            var quyen = _roleManager.Roles;
            return View(quyen);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu role đã tồn tại
                var roleExist = await _roleManager.RoleExistsAsync(model.Name);
                if (!roleExist)
                {
                    // Tạo role mới
                    var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Role đã được tạo thành công.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = "Có lỗi xảy ra khi tạo Role.";
                    }
                }
                else
                {
                    TempData["Warning"] = "Role này đã tồn tại.";
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Lấy quyền theo id
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IdentityRole model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }

                role.Name = model.Name;

                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Role đã được cập nhật thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi cập nhật Role.";
                }
            }

            return View(model);
        }



    }
}
