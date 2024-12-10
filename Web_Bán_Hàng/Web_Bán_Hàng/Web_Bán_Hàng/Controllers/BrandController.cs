using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Controllers
{
	public class BrandController : Controller
	{
		private readonly Datacontext _datacontext;
		public BrandController(Datacontext datacontext)
		{
			_datacontext = datacontext;
		}
		public async Task<IActionResult> Index(string Slug = "")
		{
			BrandModel brand = _datacontext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
			if (brand == null)
			{
				return RedirectToAction("Index");
			}
			var productByCategory = _datacontext.Products.Where(p => p.CategoryId == brand.Id);

			return View(await productByCategory.OrderByDescending(p => p.Id).ToListAsync());
		}
	}
}
