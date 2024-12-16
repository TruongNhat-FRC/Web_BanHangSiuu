using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class KhuyenMaiController : Controller
    {
        private readonly Datacontext _datacontext;

        public KhuyenMaiController(Datacontext context)
        {
            _datacontext = context;
        }
        public async Task<IActionResult> Index()
        {
            var khuyenmai = await _datacontext.KhuyenMais.ToListAsync();
            ViewBag.KhuyenMai = khuyenmai;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Add(KhuyenMaiModel khuyenmai)
        {
            if (ModelState.IsValid)
            {
                _datacontext.Add(khuyenmai);
                await _datacontext.SaveChangesAsync();
                TempData["success"] = "Thêm mã khuyến mãi thành công";
                return RedirectToAction("Index", "Brand");
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
                return BadRequest(errorMessage);
            }


            return View(khuyenmai);

        }
    }
}
