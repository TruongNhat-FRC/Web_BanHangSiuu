using Microsoft.AspNetCore.Mvc;
using Web_Bán_Hàng.Database;

namespace Web_Bán_Hàng.Controllers
{
	public class ProductController : Controller
	{
		private readonly Datacontext _datacontext;
		
		public ProductController (Datacontext context)
		{
			_datacontext = context;
		}
		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Detail(string Id = "") 
		{
			if (Id == null) return RedirectToAction("Index");
			var productbyId = _datacontext.Products.Where(p=>p.Id == Id).FirstOrDefault();


			return View(productbyId);
		}
	}
}
