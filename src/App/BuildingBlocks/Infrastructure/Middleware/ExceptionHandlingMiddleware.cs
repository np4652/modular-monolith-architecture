using System.Net;
using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace App.BuildingBlocks.Infrastructure.Middleware;

/// <summary>
/// Centralized exception handling. Expected domain failures are translated into a
/// notification for the user; anything unexpected is logged and reduced to a generic
/// error response so no stack trace, SQL, or connection detail ever reaches the client.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ITempDataDictionaryFactory _tempDataFactory;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        ITempDataDictionaryFactory tempDataFactory)
    {
        _next = next;
        _logger = logger;
        _tempDataFactory = tempDataFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain rule violation while handling {Path}", context.Request.Path);
            await HandleAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling {Path}", context.Request.Path);
            await HandleAsync(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred. Please try again.");
        }
    }

    private async Task HandleAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        if (context.Response.HasStarted)
            return;

        if (IsAjaxRequest(context))
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(new { error = message });
            return;
        }

        context.Response.Clear();
        var tempData = _tempDataFactory.GetTempData(context);
        tempData["ErrorMessage"] = message;

        var redirectTarget = statusCode == HttpStatusCode.InternalServerError
            ? "/Error"
            : context.Request.Headers.Referer.ToString() is { Length: > 0 } referer ? referer : "/";

        context.Response.Redirect(redirectTarget);
    }

    private static bool IsAjaxRequest(HttpContext context) =>
        context.Request.Headers.XRequestedWith == "XMLHttpRequest" ||
        context.Request.Headers.Accept.Any(a => a?.Contains("application/json") == true);
}
