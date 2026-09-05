using App.BuildingBlocks.Domain.Primitives;

namespace App.Modules.UserManagement.Domain.Entities;

/// <summary>
/// A single permission code granted to a <see cref="Role"/>. Owned by the Role
/// aggregate - never queried or modified outside of it.
/// </summary>
public sealed class RolePermission : Entity<Guid>
{
    private RolePermission()
    {
    }

    internal RolePermission(Guid roleId, string permissionCode) : base(Guid.NewGuid())
    {
        RoleId = roleId;
        PermissionCode = permissionCode;
    }

    public Guid RoleId { get; private init; }

    public string PermissionCode { get; private init; } = string.Empty;
}
