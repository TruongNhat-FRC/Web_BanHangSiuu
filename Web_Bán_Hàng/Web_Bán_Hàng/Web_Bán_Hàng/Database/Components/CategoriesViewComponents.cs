using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Bán_Hàng.Database.Components
{
	public class CategoriesViewComponents : ViewComponent
	{
		private readonly Datacontext _datacontext;
		public CategoriesViewComponents(Datacontext context)
		{
			_datacontext = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _datacontext.Categories.ToListAsync());
	}
}
