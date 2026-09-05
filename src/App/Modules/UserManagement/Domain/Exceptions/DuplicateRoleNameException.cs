using App.BuildingBlocks.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.Exceptions;

public sealed class DuplicateRoleNameException : DomainException
{
    public DuplicateRoleNameException(string name) : base($"A role named '{name}' already exists.")
    {
    }
}
