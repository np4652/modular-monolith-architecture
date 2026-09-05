using App.BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace App.BuildingBlocks.Infrastructure.Observability;

public sealed class HttpContextCorrelationIdProvider : ICorrelationIdProvider
{
    public const string HttpContextItemKey = "CorrelationId";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCorrelationIdProvider(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public string CorrelationId =>
        _httpContextAccessor.HttpContext?.Items[HttpContextItemKey] as string
        ?? "unavailable";
}
