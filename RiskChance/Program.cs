using Microsoft.AspNetCore.Identity;
using RiskChance.Data;
using Microsoft.EntityFrameworkCore;
using RiskChance.Models;
using Microsoft.Extensions.DependencyInjection;
using RiskChance.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<NguoiDung, IdentityRole>()
.AddDefaultTokenProviders()
.AddRoles<IdentityRole>()
 .AddDefaultUI()
 .AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache(); // Bộ nhớ tạm
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true; // Chỉ truy cập qua HTTP
    options.Cookie.IsEssential = true; // Bắt buộc cookie hoạt động
});

// Build SignalR
builder.Services.AddSignalR();

var app = builder.Build();


// Tạo Role mặc định
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new string[] { "Admin", "Founder", "Investor" };

    foreach (var role in roles)
    {
        if (!roleManager.RoleExistsAsync(role).Result)
        {
            roleManager.CreateAsync(new IdentityRole(role)).Wait();
        }
    }
}



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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "Admins",
    areaName: "Admins",
    pattern: "Admins/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Cau hinh signalR
app.MapHub<StatusStartupHub>("/statusStartupHub");

app.Run();
