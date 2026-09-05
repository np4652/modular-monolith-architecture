using App.BuildingBlocks.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.Exceptions;

public sealed class AccountLockedException : DomainException
{
    public AccountLockedException(DateTime lockedUntilUtc)
        : base($"This account is locked until {lockedUntilUtc:u}.")
    {
    }
}
