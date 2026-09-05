using System.Threading.RateLimiting;
using App.BuildingBlocks.Infrastructure.Options;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace App.BuildingBlocks.Infrastructure.RateLimiting;

/// <summary>
/// Single, centrally configured rate limiting policy for the whole application.
/// Modules must never implement their own throttling.
/// </summary>
public static class RateLimitingExtensions
{
    public const string DefaultPolicy = "default";

    public static IServiceCollection AddCentralizedRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            limiterOptions.AddPolicy(DefaultPolicy, httpContext =>
            {
                var options = httpContext.RequestServices.GetRequiredService<IOptions<RateLimitingOptions>>().Value;

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = options.PermitLimit,
                        Window = TimeSpan.FromSeconds(options.WindowSeconds),
                        QueueLimit = options.QueueLimit,
                    });
            });
        });

        return services;
    }
}
