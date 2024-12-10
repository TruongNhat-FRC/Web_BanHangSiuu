using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Database
{
	public class SeedData
	{
		public static void SeedingData(Datacontext _context)
		{
			_context.Database.Migrate();
			if (!_context.Products.Any())
			{
				CategoryModel gthn = new CategoryModel { Name = "Giáo Trính Hán Ngữ ", Slug = "giao_trinh_han_ngu", Description = "Giáo Trình Hán Ngữ Phù Hợp cho người mới bắt đầu học tiếng Trung", Status = 1 };
				CategoryModel gtc  = new CategoryModel { Name = "Giáo Trình Chuẩn HSK ", Slug = "giao_trinh_chuan_hsk", Description = "Giáo Trình Chuẩn HSK Phù Hợp cho người mới bắt đầu học tiếng Trung", Status = 1 };
				BrandModel gthn1 = new BrandModel { Name = "Giáo Trính Hán Ngữ ", Slug = "giao_trinh_han_ngu", Description = "Giáo Trình Hán Ngữ Phù Hợp cho người mới bắt đầu học tiếng Trung", status = 1 };
				BrandModel gtc1 = new BrandModel { Name = "Giáo Trình Chuẩn HSK ", Slug = "giao_trinh_chuan_hsk", Description = "Giáo Trình Chuẩn HSK Phù Hợp cho người mới bắt đầu học tiếng Trung", status = 1 };
				_context.Products.AddRange(

					new ProductModel { Id = "1", Name = "Giáo Trình Hán Ngữ 1", Slug = "giao_trinh_han_ngu_1", Description = "Giáo trình hán ngữ 1, cực kỳ uy tín", Image = "1.jpg", Category = gthn, Brand = gthn1, Price = 699 },
					new ProductModel { Id = "2", Name = "Giáo Trình Chuẩn HSK 1", Slug = "giao_trinh_chuan_hsk_1", Description = "Giáo trình chuẩn hsk, cực kỳ uy tín", Image = "2.jpg", Category = gtc, Brand = gtc1, Price = 799 }
				);
				_context.SaveChanges();

			}
		}
	}
}
