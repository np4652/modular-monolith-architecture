using App.BuildingBlocks.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.Exceptions;

public sealed class InvalidRoleNameException : DomainException
{
    public InvalidRoleNameException() : base("A role name cannot be empty.")
    {
    }
}
