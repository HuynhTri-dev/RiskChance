    using Microsoft.AspNetCore.Identity;
    using RiskChance.Data;
    using RiskChance.Models;

namespace RiskChance.Middleware
{
    public class AccessLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AccessLogMiddleware> _logger;

        public AccessLogMiddleware(RequestDelegate next, ILogger<AccessLogMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));  // Đảm bảo next không phải là null
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDBContext db, UserManager<NguoiDung> userManager)
        {
            try
            {
                var user = context.User;

                if (user.Identity.IsAuthenticated &&
                    (user.IsInRole("Founder") || user.IsInRole("Investor")))
                {
                    var currentUser = await userManager.GetUserAsync(user);

                    // Kiểm tra đã ghi log hôm nay chưa
                    var today = DateTime.Today;
                    var hasLogToday = db.AccessLogs.Any(log =>
                        log.UserId == currentUser.Id && log.AccessTime.Date == today);

                    if (!hasLogToday)
                    {
                        var log = new AccessLog
                        {
                            IPAddress = context.Connection.RemoteIpAddress?.ToString(),
                            UserAgent = context.Request.Headers["User-Agent"].ToString(),
                            AccessTime = DateTime.Now,
                            UserId = currentUser.Id
                        };

                        db.AccessLogs.Add(log);
                        await db.SaveChangesAsync();
                    }
                }

                // Kiểm tra _next trước khi gọi
                if (_next != null)
                {
                    await _next(context); // Gọi middleware tiếp theo trong pipeline
                }
                else
                {
                    _logger.LogError("RequestDelegate _next is null in AccessLogMiddleware.");
                    context.Response.StatusCode = 500; // Trả về lỗi Server nếu _next là null
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi ghi AccessLog.");
            }
        }
    }
}