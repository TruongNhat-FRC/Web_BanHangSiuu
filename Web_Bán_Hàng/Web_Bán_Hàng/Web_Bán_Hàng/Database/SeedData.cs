using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Database
{
    public class SeedData
    {
        public static void SeedingData(Datacontext _context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context.Database.Migrate();

            if (!_context.Products.Any())
            {
                CategoryModel gthn = new CategoryModel { Name = "Giáo Trính Hán Ngữ", Slug = "giao_trinh_han_ngu", Description = "Giáo Trình Hán Ngữ Phù Hợp cho người mới bắt đầu học tiếng Trung", Status = 1 };
                CategoryModel gtc = new CategoryModel { Name = "Giáo Trình Chuẩn HSK", Slug = "giao_trinh_chuan_hsk", Description = "Giáo Trình Chuẩn HSK Phù Hợp cho người mới bắt đầu học tiếng Trung", Status = 1 };
                BrandModel gthn1 = new BrandModel { Name = "Giáo Trình Hán Ngữ", Slug = "giao_trinh_han_ngu", Description = "Giáo Trình Hán Ngữ Phù Hợp cho người mới bắt đầu học tiếng Trung", status = 1 };
                BrandModel gtc1 = new BrandModel { Name = "Giáo Trình Chuẩn HSK", Slug = "giao_trinh_chuan_hsk", Description = "Giáo Trình Chuẩn HSK Phù Hợp cho người mới bắt đầu học tiếng Trung", status = 1 };

                _context.Products.AddRange(
                    new ProductModel { Id = "GTHN", Name = "Giáo Trình Hán Ngữ 1", Slug = "giao_trinh_han_ngu_1", Description = "Giáo trình hán ngữ 1, cực kỳ uy tín", Image = "83670eb2-6ed7-465b-a6a9-25eea669ae99_cover051.png", Category = gthn, Brand = gthn1, Price = 699 },
                    new ProductModel { Id = "GTHSK", Name = "Giáo Trình Chuẩn HSK 1", Slug = "giao_trinh_chuan_hsk_1", Description = "Giáo trình chuẩn hsk, cực kỳ uy tín", Image = "83670eb2-6ed7-465b-a6a9-25eea669ae99_cover051.png", Category = gtc, Brand = gtc1, Price = 799 }
                );
                _context.SaveChanges();
            }
            // Thêm dữ liệu liên hệ
            if (!_context.LienHes.Any())
            {
                var contact = new LienHeModel
                {
                    Name = "Shop Sách FRC",
                    BanDo = "<iframe src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3898.7063016443135!2d109.19980097482907!3d12.268143687986354!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317067ed3a052f11%3A0xd464ee0a6e53e8b7!2zVHLGsOG7nW5nIMSQ4bqhaSBo4buNYyBOaGEgVHJhbmc!5e0!3m2!1svi!2s!4v1734148705514!5m2!1svi!2s\" width=\"400\" height=\"400\" style=\"border:0;\" allowfullscreen=\"\" loading=\"lazy\" referrerpolicy=\"no-referrer-when-downgrade\"></iframe>",
                    Email = "truong.pn.64cntt@ntu.edu.vn",
                    SDT = "0377875295",
                    Mota = "Shop sách FRC là một cửa hàng sách giả tưởng chuyên cung cấp các loại sách đa dạng từ truyện tranh, tiểu thuyết, sách học thuật cho đến sách thiếu nhi. Với tiêu chí \"Mỗi cuốn sách là một chuyến hành trình\", FRC mang đến trải nghiệm mua sắm thân thiện, dễ dàng, và tiện lợi. Ngoài ra, FRC còn hỗ trợ khách hàng đặt hàng trực tuyến và giao hàng nhanh, giúp mọi người dễ dàng sở hữu những cuốn sách yêu thích. Dù là khám phá tri thức hay tìm kiếm niềm vui, FRC luôn là nơi đồng hành tuyệt vời cho mọi tín đồ yêu sách! 📚",
                    Logo = "412feed4-f1a3-4f43-bd01-0dd63b1ccf64_a-bookstore-logo-with-the-text-frc-store_JS0ryOxJT0WEnF-zJDr6Dw_N_4rOpaSRnmeoVpFk6EY6w.jpeg"
                };

                _context.LienHes.Add(contact);
                _context.SaveChanges();
            }

            // Seeding vai trò
            SeedRoles(roleManager);

            // Seeding người dùng admin mặc định
            SeedUser(userManager, roleManager).Wait();
        }

        // Seeding Vai Trò
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "NguoiDung", "NhanVien" };

            foreach (var roleName in roleNames)
            {
                var roleExist = roleManager.RoleExistsAsync(roleName).Result;
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    var result = roleManager.CreateAsync(role).Result;
                }
            }
        }

        // Seeding Người Dùng Admin Mặc Định
        public static async Task SeedUser(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@domain.com";
            string adminPassword = "Admin123@";

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new AppUserModel
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
