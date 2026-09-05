namespace App.BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Base type for exceptions that represent a violated business invariant.
/// Translated by the presentation layer into validation/notification messages,
/// never exposed to the client as a raw stack trace.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
