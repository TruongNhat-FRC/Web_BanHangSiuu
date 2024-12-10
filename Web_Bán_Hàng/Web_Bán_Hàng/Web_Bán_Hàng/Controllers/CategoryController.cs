using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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



		public async Task<IActionResult> Index(string Slug = "")
		{
			CategoryModel category = _datacontext.Categories.Where(c=>c.Slug == Slug).FirstOrDefault();
			if (category == null) { 
				return RedirectToAction("Index");
			}
			var productByCategory = _datacontext.Products.Where(p => p.CategoryId == category.Id);
			
			return View(await productByCategory.OrderByDescending(p =>p.Id).ToListAsync());
		}
	}
}
