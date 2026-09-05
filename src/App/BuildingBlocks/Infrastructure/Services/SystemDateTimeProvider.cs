using App.BuildingBlocks.Application.Abstractions;

namespace App.BuildingBlocks.Infrastructure.Services;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
