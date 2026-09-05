using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Infrastructure.Authorization;
using App.BuildingBlocks.Infrastructure.Persistence;
using App.Modules.UserManagement.Application;
using App.Modules.UserManagement.Application.EventHandlers;
using App.Modules.UserManagement.Application.Pipelines.Authentication;
using App.Modules.UserManagement.Application.Services;
using App.Modules.UserManagement.Application.Validators;
using App.Modules.UserManagement.Domain.Events;
using App.Modules.UserManagement.Domain.Repositories;
using App.Modules.UserManagement.Infrastructure.Persistence;
using App.Modules.UserManagement.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.UserManagement;

/// <summary>
/// Composition entry point for the UserManagement module. Program.cs calls this and
/// nothing else to bring the whole module online - it never reaches into the
/// module's internals itself.
/// </summary>
public static class UserManagementModule
{
    public static IServiceCollection AddUserManagementModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UserManagementDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("UserManagement"));
            options.AddInterceptors(
                sp.GetRequiredService<AuditingInterceptor>(),
                sp.GetRequiredService<SoftDeleteInterceptor>(),
                sp.GetRequiredService<DomainEventDispatchInterceptor>());
        });

        services.AddScoped<AuditingInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<DomainEventDispatchInterceptor>();

        services.AddScoped<IUnitOfWork, EfUnitOfWork<UserManagementDbContext>>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped<CheckAccountLockoutStep>();
        services.AddScoped<ValidateCredentialsStep>();
        services.AddScoped<CheckAccountStatusStep>();
        services.AddScoped<CheckMfaIfEnabledStep>();
        services.AddScoped<GenerateAuthenticationResultStep>();
        services.AddScoped<AuthenticationPipeline>();

        services.AddScoped<IDomainEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();
        services.AddScoped<IDomainEventHandler<UserLockedOutEvent>, UserLockedOutEventHandler>();

        services.AddSingleton<IPermissionCatalogContributor, UserManagementPermissionCatalogContributor>();

        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>(
            filter: result => result.ValidatorType.Namespace?
                .StartsWith("App.Modules.UserManagement.Application.Validators", StringComparison.Ordinal) == true);

        return services;
    }
}
