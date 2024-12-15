using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Database
{
    public class Datacontext : IdentityDbContext<AppUserModel>
    {
        public Datacontext(DbContextOptions<Datacontext> options) : base(options)
        {
        }

        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<DonHangModel> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<GioHangSave> GioHang { get; set; }
        public DbSet<CartHistoryModel> LichSuGioHang { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<LienHeModel> LienHes { get; set; }
        public DbSet<YeuThichModel> YeuThichs { get; set; }
        public DbSet<SoSanhModel> SoSanhs { get; set; }
        public DbSet<VanChuyenModel> VanChuyens { get; set; }   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Gọi base để Identity tự cấu hình các bảng của nó
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng ProductModel (ví dụ: định dạng kiểu dữ liệu)
            modelBuilder.Entity<ProductModel>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Cấu hình bảng DonHangModel
            modelBuilder.Entity<DonHangModel>()
                .HasIndex(d => d.MaDonHang)
                .IsUnique(); // Đảm bảo MaDonHang là duy nhất

            // Cấu hình bảng ChiTietDonHang
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(ct => ct.DonHang) // Liên kết với DonHangModel
                .WithMany(dh => dh.ChiTietDonHangs) // DonHangModel có nhiều ChiTietDonHang
                .HasForeignKey(ct => ct.MaDonHang) // Khóa ngoại là MaDonHang
                .HasPrincipalKey(dh => dh.MaDonHang) // Khóa chính liên kết là MaDonHang (có ràng buộc UNIQUE)
                .OnDelete(DeleteBehavior.Cascade); // Xóa DonHang sẽ xóa luôn ChiTietDonHang

            // Cấu hình bảng ProductModel (nếu cần)
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(ct => ct.Product) // Liên kết với ProductModel
                .WithMany() // Không cần quan hệ ngược trong ProductModel
                .HasForeignKey(ct => ct.MaSanPham) // Khóa ngoại là MaSanPham
                .OnDelete(DeleteBehavior.Restrict); // Hạn chế xóa sản phẩm nếu có liên kết
        }

    }
}
