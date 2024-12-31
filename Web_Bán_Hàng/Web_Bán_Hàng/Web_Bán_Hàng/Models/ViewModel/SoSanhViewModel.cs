namespace Web_Bán_Hàng.Models.ViewModel
{
    public class SoSanhViewModel
    {
        public int Id { get; set; }
        public string ProductId { get; set; }       
        public string ProductName { get; set; }     
        public string Description { get; set; }     
        public decimal Price { get; set; }          
        public string Image { get; set; }          
        public string UserName { get; set; }       
        public string UserId { get; set; }         
        public bool TrangThai { get; set; }
    }
}
