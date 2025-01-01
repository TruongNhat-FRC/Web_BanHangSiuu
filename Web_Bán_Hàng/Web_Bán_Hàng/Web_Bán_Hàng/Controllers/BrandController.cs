using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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
		public async Task<IActionResult> Index(string Slug = "", string sort_by = "", int pg = 1)
		{
			BrandModel brand = _datacontext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
			if (brand == null)
			{
				return RedirectToAction("Index");
			}
			ViewBag.Name = brand.Name;

			IQueryable<ProductModel> productByBrand = _datacontext.Products.Where(p => p.BrandId == brand.Id);

			if (!string.IsNullOrEmpty(sort_by))
			{
				if (sort_by == "price_increase")
				{
					productByBrand = productByBrand.OrderBy(p => p.Price);
				}
				else if (sort_by == "price_decrease")
				{
					productByBrand = productByBrand.OrderByDescending(p => p.Price);
				}
				else if (sort_by == "price_newest")
				{
					productByBrand = productByBrand.OrderByDescending(p => p.Id);
				}
				else if (sort_by == "price_oldest")
				{
					productByBrand = productByBrand.OrderBy(p => p.Id);
				}
				else if (sort_by == "count_oldest")
				{
					productByBrand = productByBrand.OrderByDescending(p => p.PurchaseCount);
				}
				else
				{
					productByBrand = productByBrand.OrderByDescending(p => p.Id);
				}
			}

			int count = await productByBrand.CountAsync();

			const int KichThuoc = 6;
			if (pg < 1)
			{
				pg = 1;
			}

			var trang = new PhanTrang(count, pg, KichThuoc);

			int buocnhay = (pg - 1) * KichThuoc;

			var data = await productByBrand.Skip(buocnhay).Take(trang.KichThuocTrang).ToListAsync();

			ViewBag.Trang = trang;

			return View(data);
		}

	}
}
