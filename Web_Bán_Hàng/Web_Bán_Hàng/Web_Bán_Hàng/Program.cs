using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Web_Bán_Hàng.Areas.Repository;
using Web_Bán_Hàng.Database;
using Web_Bán_Hàng.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Datacontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDb")));

// Add services to the container.
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.IsEssential = true;
});
builder.Services.AddIdentity<AppUserModel, IdentityRole>()
    .AddEntityFrameworkStores<Datacontext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    /*options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;*/

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(options =>
    {
        options.AccessDeniedPath = "/Account/AccessDenied"; 
        options.LoginPath = "/Account/DangNhap"; 
        options.LogoutPath = "/Account/DangXuat"; 
    });

var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode=404");
app.UseSession();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area}/{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "category",
    pattern: "/category/{Slug?}",
    defaults: new { controller = "Category", action = "Index" }
);
app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{Slug?}",
    defaults: new { controller = "Brand", action = "Index" }
);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//Seedng Data
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<Datacontext>();
SeedData.SeedingData(context);

app.Run();
