using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace App.BuildingBlocks.Infrastructure.Observability;

/// <summary>
/// Composition-root wiring for structured logging and distributed tracing.
/// Centralizes the two cross-cutting observability concerns so no module has to.
/// </summary>
public static class ObservabilityExtensions
{
    public static void UseSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console());
    }

    public static IServiceCollection AddApplicationOpenTelemetry(
        this IServiceCollection services,
        string serviceName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter());

        return services;
    }
}
