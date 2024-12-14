using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_Bán_Hàng.Database.Validation;

namespace Web_Bán_Hàng.Models
{
    public class SliderModel
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        public string Mota {  get; set; }
        public int Status {  get; set; }
        public string Image {  get; set; }
        [NotMapped]
        [CheckFile]
        public IFormFile ImageUpLoad { get; set; }
    }
}
