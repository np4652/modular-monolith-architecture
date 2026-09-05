using App.BuildingBlocks.Domain.Abstractions;
using App.BuildingBlocks.Domain.Primitives;
using App.Modules.UserManagement.Domain.Exceptions;

namespace App.Modules.UserManagement.Domain.Entities;

public sealed class Role : AggregateRoot<Guid>, IAuditableEntity
{
    private readonly List<RolePermission> _permissions = [];

    private Role()
    {
    }

    private Role(string name, string? description) : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsSystemRole { get; private init; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    public DateTime CreatedAtUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTime? ModifiedAtUtc { get; private set; }

    public string? ModifiedBy { get; private set; }

    public static Role Create(string name, string? description = null) => new(RequireName(name), description);

    public static Role CreateSystemRole(string name, string? description, IEnumerable<string> permissionCodes)
    {
        var role = new Role(RequireName(name), description) { IsSystemRole = true };
        foreach (var code in permissionCodes)
            role.GrantPermission(code);

        return role;
    }

    public void Rename(string name) => Name = RequireName(name);

    public void UpdateDescription(string? description) => Description = description;

    public void GrantPermission(string permissionCode)
    {
        if (_permissions.Any(p => p.PermissionCode == permissionCode))
            return;

        _permissions.Add(new RolePermission(Id, permissionCode));
    }

    public void RevokePermission(string permissionCode) =>
        _permissions.RemoveAll(p => p.PermissionCode == permissionCode);

    public void ReplacePermissions(IEnumerable<string> permissionCodes)
    {
        _permissions.Clear();
        foreach (var code in permissionCodes.Distinct())
            _permissions.Add(new RolePermission(Id, code));
    }

    public void SetCreated(DateTime occurredOnUtc, string? actor)
    {
        CreatedAtUtc = occurredOnUtc;
        CreatedBy = actor;
    }

    public void SetModified(DateTime occurredOnUtc, string? actor)
    {
        ModifiedAtUtc = occurredOnUtc;
        ModifiedBy = actor;
    }

    private static string RequireName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidRoleNameException();

        return name.Trim();
    }
}
