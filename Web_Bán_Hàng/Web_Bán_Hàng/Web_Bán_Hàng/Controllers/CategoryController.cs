using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Controllers
{
	public class CategoryController : Controller
	{
		private readonly Datacontext _datacontext;
		public CategoryController(Datacontext context)
		{
			_datacontext = context;
		}



		public async Task<IActionResult> Index(string Slug = "", string sort_by = "", int pg = 1)
		{
			CategoryModel category = _datacontext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
			if (category == null)
			{
				return RedirectToAction("Index");
			}

			IQueryable<ProductModel> productByCategory = _datacontext.Products.Where(p => p.CategoryId == category.Id);
			ViewBag.Name = category.Name;

			var count = await productByCategory.CountAsync();
			if (count > 0)
			{
				if (sort_by == "price_increase")
				{
					productByCategory = productByCategory.OrderBy(p => p.Price);
				}
				else if (sort_by == "price_decrease")
				{
					productByCategory = productByCategory.OrderByDescending(p => p.Price);
				}
				else if (sort_by == "price_newest")
				{
					productByCategory = productByCategory.OrderByDescending(p => p.Id);
				}
				else if (sort_by == "price_oldest")
				{
					productByCategory = productByCategory.OrderBy(p => p.Id);
				}
                else if (sort_by == "count_oldest")
                {
                    productByCategory = productByCategory.OrderByDescending(p => p.PurchaseCount);
                }
                else
				{
					productByCategory = productByCategory.OrderByDescending(p => p.Id);
				}
			}

			const int KichThuoc = 6; 
			if (pg < 1)
			{
				pg = 1;
			}

			var trang = new PhanTrang(count, pg, KichThuoc);

			int buocnhay = (pg - 1) * KichThuoc;

			var data = await productByCategory.Skip(buocnhay).Take(trang.KichThuocTrang).ToListAsync();

			ViewBag.Trang = trang;

			return View(data);
		}

	}
}
