using Microsoft.AspNetCore.Authentication.Cookies;

namespace App.BuildingBlocks.Infrastructure.Authentication;

/// <summary>
/// The single, centrally configured browser authentication scheme for the whole
/// application. Modules never register their own authentication handler.
/// </summary>
public static class CookieAuthenticationExtensions
{
    public static IServiceCollection AddApplicationCookieAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "App.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

        return services;
    }
}
