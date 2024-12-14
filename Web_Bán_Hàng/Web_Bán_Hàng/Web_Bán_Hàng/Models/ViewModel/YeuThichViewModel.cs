namespace Web_Bán_Hàng.Models.ViewModel
{
    public class YeuThichViewModel
    {
        public int Id { get; set; }
        public string ProductId { get; set; }       // Mã sản phẩm
        public string ProductName { get; set; }     // Tên sản phẩm
        public string Description { get; set; }     // Mô tả sản phẩm
        public decimal Price { get; set; }          // Giá sản phẩm
        public string Image { get; set; }           // Đường dẫn hình ảnh sản phẩm
        public string UserName { get; set; }        // Tên người dùng
        public string UserId { get; set; }          // Mã người dùng
        public bool TrangThai {  get; set; }
    }
}
