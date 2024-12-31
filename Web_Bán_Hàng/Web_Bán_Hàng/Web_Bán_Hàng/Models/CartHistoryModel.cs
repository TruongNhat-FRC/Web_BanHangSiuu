using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Bán_Hàng.Models
{
    public class CartHistoryModel
    {
        [Key]
        public int Id { get; set; } 

        public string UserId { get; set; } 
        public string ProductId { get; set; } 
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get { return Quantity * Price; } }
        public string Image { get; set; }
        public DateTime AddedDate { get; set; } 
        public bool IsCheckedOut { get; set; } 
       
        [ForeignKey("UserId")]
        public virtual AppUserModel User { get; set; } 

        [ForeignKey("ProductId")]
        public virtual ProductModel Product { get; set; } 
    }
}
