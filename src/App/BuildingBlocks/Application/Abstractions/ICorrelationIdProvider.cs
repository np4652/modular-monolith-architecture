namespace App.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Makes the current request's correlation id available to PageModels, Application
/// services, Infrastructure and logs without any of them reaching into HttpContext.
/// </summary>
public interface ICorrelationIdProvider
{
    string CorrelationId { get; }
}
