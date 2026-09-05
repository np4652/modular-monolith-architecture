namespace App.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Implemented by entities that must be soft-deleted rather than physically removed.
/// Enforced centrally by an EF query filter/interceptor.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }

    DateTime? DeletedAtUtc { get; }

    void MarkDeleted(DateTime occurredOnUtc);
}
