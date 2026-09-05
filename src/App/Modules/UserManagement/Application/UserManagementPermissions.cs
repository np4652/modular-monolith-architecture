using App.BuildingBlocks.Infrastructure.Authorization;

namespace App.Modules.UserManagement.Application;

/// <summary>
/// The permission vocabulary owned by this module. Presentation depends on these
/// constants (never on raw strings); Infrastructure seeds them onto the built-in
/// roles; the module registration contributes them to the shared permission registry.
/// </summary>
public static class UserManagementPermissions
{
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersEdit = "users.edit";
    public const string UsersDeactivate = "users.deactivate";
    public const string RolesView = "roles.view";
    public const string RolesManage = "roles.manage";

    public static IReadOnlyList<PermissionDescriptor> All { get; } =
    [
        new(UsersView, "Users", "View users"),
        new(UsersCreate, "Users", "Create users"),
        new(UsersEdit, "Users", "Edit users"),
        new(UsersDeactivate, "Users", "Activate/deactivate users"),
        new(RolesView, "Roles", "View roles"),
        new(RolesManage, "Roles", "Manage roles and permissions"),
    ];
}
