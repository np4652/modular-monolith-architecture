using App.BuildingBlocks.Domain.Primitives;

namespace App.Modules.UserManagement.Domain.ValueObjects;

/// <summary>
/// Wraps an already-hashed password. The domain never sees, and never hashes,
/// a plain text password - that happens through IPasswordHasher in Application.
/// </summary>
public sealed class PasswordHash : ValueObject
{
    private PasswordHash(string value) => Value = value;

    public string Value { get; }

    public static PasswordHash FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("A password hash cannot be empty.", nameof(hash));

        return new PasswordHash(hash);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
