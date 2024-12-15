namespace Web_Bán_Hàng.Models.ViewModel
{
	public class CartItemViewModel
	{
		public List<CartItemModel> CartItem { get; set; }
		public decimal GrandTotal {  get; set; }
		public decimal Phiship {  get; set; }	
	}
}
