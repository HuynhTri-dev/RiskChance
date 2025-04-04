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
            _next = next;
            _logger = logger;
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi ghi AccessLog.");
            }

            await _next(context);
        }
    }
}
