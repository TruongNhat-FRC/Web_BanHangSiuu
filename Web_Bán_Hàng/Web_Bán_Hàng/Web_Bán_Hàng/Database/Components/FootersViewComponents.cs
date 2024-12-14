using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Bán_Hàng.Database.Components
{

	public class FootersViewComponents : ViewComponent
	{
		private readonly Datacontext _datacontext;
		public FootersViewComponents(Datacontext context)
		{
			_datacontext = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _datacontext.LienHes.FirstOrDefaultAsync());
	}
}
