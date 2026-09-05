using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Infrastructure.Authentication;
using App.BuildingBlocks.Infrastructure.Authorization;
using App.BuildingBlocks.Infrastructure.Caching;
using App.BuildingBlocks.Infrastructure.Events;
using App.BuildingBlocks.Infrastructure.Messaging;
using App.BuildingBlocks.Infrastructure.Middleware;
using App.BuildingBlocks.Infrastructure.Observability;
using App.BuildingBlocks.Infrastructure.Options;
using App.BuildingBlocks.Infrastructure.RateLimiting;
using App.BuildingBlocks.Infrastructure.Security;
using App.BuildingBlocks.Infrastructure.Services;
using App.Modules.UserManagement;
using App.Modules.UserManagement.Application;
using App.Modules.UserManagement.Infrastructure.Persistence;
using App.Modules.UserManagement.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.UseSerilogLogging();

// ----- Options -----
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.SectionName));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.Configure<RateLimitingOptions>(builder.Configuration.GetSection(RateLimitingOptions.SectionName));

// ----- Cross-cutting BuildingBlocks -----
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();
builder.Services.AddScoped<ICorrelationIdProvider, HttpContextCorrelationIdProvider>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IDomainEventDispatcher, InProcessDomainEventDispatcher>();

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetSection(RedisOptions.SectionName)["ConnectionString"]
                                   ?? "localhost:6379"));
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

builder.Services.AddSingleton<IPermissionRegistry, PermissionRegistry>();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddApplicationCookieAuthentication();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
builder.Services.AddCentralizedRateLimiting();
builder.Services.AddApplicationOpenTelemetry(serviceName: "App");

// ----- Modules -----
builder.Services.AddUserManagementModule(builder.Configuration);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Users", PermissionPolicyProvider.PolicyPrefix + UserManagementPermissions.UsersView);
    options.Conventions.AuthorizeFolder("/Roles", PermissionPolicyProvider.PolicyPrefix + UserManagementPermissions.RolesView);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

if (builder.Configuration.GetValue("Database:AutoMigrate", true))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
    await context.Database.MigrateAsync();
    await RoleSeeder.SeedAsync(context);
    await AdminUserSeeder.SeedAsync(
        context,
        scope.ServiceProvider.GetRequiredService<IConfiguration>(),
        scope.ServiceProvider.GetRequiredService<IPasswordHasher>());
}

app.Run();

public partial class Program;
