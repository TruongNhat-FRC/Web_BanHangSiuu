using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Bán_Hàng.Database.Components
{
	public class BrandsViewComponents : ViewComponent
	{
		private readonly Datacontext _datacontext;
		public BrandsViewComponents(Datacontext context)
		{
			_datacontext = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _datacontext.Brands.ToListAsync());
	}
}
