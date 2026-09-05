using App.BuildingBlocks.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.Exceptions;

public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() : base("The email or password is incorrect.")
    {
    }
}
