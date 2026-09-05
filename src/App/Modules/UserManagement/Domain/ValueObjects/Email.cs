using System.Text.RegularExpressions;
using App.BuildingBlocks.Domain.Primitives;
using App.Modules.UserManagement.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    private Email(string value) => Value = value;

    public string Value { get; }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException(value);

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailPattern().IsMatch(normalized))
            throw new InvalidEmailException(value);

        return new Email(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();
}
