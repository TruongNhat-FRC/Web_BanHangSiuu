using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Bán_Hàng.Models
{
    public class CartHistoryModel
    {
        [Key]
        public int Id { get; set; } // Primary Key

        public string UserId { get; set; } // Foreign Key: UserId
        public string ProductId { get; set; } // Foreign Key: ProductId
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get { return Quantity * Price; } }
        public string Image { get; set; }
        public DateTime AddedDate { get; set; } // Thời gian sản phẩm được thêm vào giỏ hàng
        public bool IsCheckedOut { get; set; } // Trạng thái thanh toán

        // Navigation properties with virtual for Lazy Loading
        [ForeignKey("UserId")]
        public virtual AppUserModel User { get; set; } // Liên kết đến User (nếu sử dụng Identity)

        [ForeignKey("ProductId")]
        public virtual ProductModel Product { get; set; } // Liên kết đến Product (nếu có bảng sản phẩm)
    }
}
