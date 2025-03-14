// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RiskChance.Models;

namespace RiskChance.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<NguoiDung> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost()
        {
            // Xóa toàn bộ thông tin phiên làm việc
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);

            // Xóa toàn bộ session
            //HttpContext.Session.Clear();

            // Đăng xuất khỏi hệ thống Identity
            await _signInManager.SignOutAsync();

            // Chuyển hướng về trang đăng nhập ngay lập tức
            return RedirectToPage("/Account/Login");

            //await _signInManager.SignOutAsync();
            //_logger.LogInformation("User logged out.");
            //if (returnUrl != null)
            //{
            //    return LocalRedirect(returnUrl);
            //}
            //else
            //{
            //    // This needs to be a redirect so that the browser performs a new
            //    // request and the identity for the user gets updated.
            //    return RedirectToPage();
            //}
        }
    }
}
