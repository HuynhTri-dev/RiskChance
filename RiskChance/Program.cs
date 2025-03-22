using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Hubs;
using RiskChance.Models;
using RiskChance.Repositories;

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

// Gây ra hiện tượng bị đè dữ liệu khi chung 1 tab
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build SignalR
builder.Services.AddSignalR();

//builder.WebHost.UseUrls("http://0.0.0.0:7078");

builder.Services.AddScoped(typeof(IRepository<>), typeof(TRepository<>));

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

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


// sử dụng pattern : là tên đường dẫn. còn defaults: new {} : là tên điwonfg dẫn
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller}/{action}/{id?}"
    );

    //endpoints.MapControllerRoute(
    //    name: "DetailStartup",
    //    pattern: "Details/{name:alpha}",
    //    defaults: new { controller = "Startup", action = "Details" }
    //);

    // Mạc định
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapHub<StatusStartupHub>("/statusStartupHub").RequireAuthorization();
    endpoints.MapHub<PostCommentStartupHub>("/postCommentStartupHub").RequireAuthorization();
});

app.MapRazorPages();


app.Run();
