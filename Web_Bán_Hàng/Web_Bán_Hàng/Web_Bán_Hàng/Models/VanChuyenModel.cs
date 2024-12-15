using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Models
{
    public class VanChuyenModel
    {
        [Key]
        public int Id { get; set; }
        public decimal Gia {  get; set; }
        public string Phuong_xa {  get; set; }
        public string Quan {  get; set; }
        public string thanhpho {  get; set; }
    }
}
