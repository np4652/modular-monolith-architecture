using App.BuildingBlocks.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.Exceptions;

public sealed class AccountNotActiveException : DomainException
{
    public AccountNotActiveException() : base("This account is not active.")
    {
    }
}
