using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Hubs;
using RiskChance.Models;
using RiskChance.Repositories;
using RiskChance.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<NguoiDung, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build SignalR
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024;  // 10 MB
});

//builder.WebHost.UseUrls("http://0.0.0.0:7078");

builder.Services.AddScoped(typeof(IRepository<>), typeof(TRepository<>));
builder.Services.AddScoped<NotificationService>();

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

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//Thêm Middleware
app.UseRouting();


app.UseSession();   
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AccessLogMiddleware>();

// sử dụng pattern : là tên đường dẫn. còn defaults: new {} : là tên điwonfg dẫn
// Định tuyến cho các khu vực (Areas)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller}/{action}/{id?}"
);

// Định tuyến riêng cho HopDong
app.MapControllerRoute(
    name: "HopDongRoute",
    pattern: "HopDong/{idStartup:int}",
    defaults: new { area = "Investor", controller = "HopDong", action = "Create" }
);

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<StatusStartupHub>("/statusStartupHub");
app.MapHub<StatusStartupHub>("/statusNewsHub");
app.MapHub<PostCommentStartupHub>("/postCommentStartupHub");
app.MapHub<PostCommentNewsHub>("/postCommentNewsHub");
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<MessengerHub>("/messengerHub");

app.MapRazorPages();


app.Run();
