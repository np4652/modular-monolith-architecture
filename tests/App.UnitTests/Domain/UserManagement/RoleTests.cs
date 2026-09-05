using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.Exceptions;

namespace App.UnitTests.Domain.UserManagement;

public class RoleTests
{
    [Fact]
    public void Create_Throws_WhenNameIsBlank()
    {
        Assert.Throws<InvalidRoleNameException>(() => Role.Create("   "));
    }

    [Fact]
    public void GrantPermission_IsIdempotent()
    {
        var role = Role.Create("Editor");

        role.GrantPermission("content.edit");
        role.GrantPermission("content.edit");

        Assert.Single(role.Permissions);
    }

    [Fact]
    public void ReplacePermissions_RemovesAnythingNotInTheNewSet()
    {
        var role = Role.Create("Editor");
        role.GrantPermission("content.edit");
        role.GrantPermission("content.delete");

        role.ReplacePermissions(["content.view"]);

        var code = Assert.Single(role.Permissions);
        Assert.Equal("content.view", code.PermissionCode);
    }

    [Fact]
    public void CreateSystemRole_GrantsEveryRequestedPermission()
    {
        var role = Role.CreateSystemRole("Administrator", "Full access", ["a", "b", "c"]);

        Assert.True(role.IsSystemRole);
        Assert.Equal(3, role.Permissions.Count);
    }
}
