using System.ComponentModel.DataAnnotations;

namespace Web_Bán_Hàng.Database.Validation
{
    public class CheckFile : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
{
    if (value is IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName); //123.jpg
        string[] extensions = { "jpg", "png", "jpeg" };

        bool result = extensions.Any(x => extension.EndsWith(x));

        if (!result)
        {
            return new ValidationResult("Cho phép các loại ảnh như jpg ,png ,jpeg");
        }
    }
    return ValidationResult.Success;
}

    }
}
