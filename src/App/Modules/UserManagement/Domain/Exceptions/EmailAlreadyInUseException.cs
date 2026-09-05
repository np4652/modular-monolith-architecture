using App.BuildingBlocks.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.Exceptions;

public sealed class EmailAlreadyInUseException : DomainException
{
    public EmailAlreadyInUseException(string email) : base($"The email '{email}' is already registered.")
    {
    }
}
