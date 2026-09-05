using App.BuildingBlocks.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.Exceptions;

public sealed class InvalidEmailException : DomainException
{
    public InvalidEmailException(string value) : base($"'{value}' is not a valid email address.")
    {
    }
}
