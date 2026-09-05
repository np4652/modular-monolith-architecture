using App.BuildingBlocks.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.Exceptions;

public sealed class SystemRoleImmutableException : DomainException
{
    public SystemRoleImmutableException(string roleName)
        : base($"'{roleName}' is a built-in role and cannot be changed or deleted.")
    {
    }
}
