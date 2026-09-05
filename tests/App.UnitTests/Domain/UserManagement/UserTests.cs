using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.Enums;
using App.Modules.UserManagement.Domain.Events;
using App.Modules.UserManagement.Domain.ValueObjects;

namespace App.UnitTests.Domain.UserManagement;

public class UserTests
{
    private static User NewUser() =>
        User.Register(Email.Create("user@example.com"), PasswordHash.FromHash("hashed-value"));

    [Fact]
    public void Register_RaisesUserRegisteredEvent_AndDefaultsToActive()
    {
        var user = NewUser();

        Assert.Equal(UserStatus.Active, user.Status);
        var domainEvent = Assert.Single(user.DomainEvents);
        Assert.IsType<UserRegisteredEvent>(domainEvent);
    }

    [Fact]
    public void RecordFailedLoginAttempt_LocksAccount_AfterMaxAttempts()
    {
        var user = NewUser();
        var now = DateTime.UtcNow;

        for (var i = 0; i < User.MaxFailedLoginAttempts - 1; i++)
        {
            user.RecordFailedLoginAttempt(now);
            Assert.False(user.IsLockedOut(now));
        }

        user.RecordFailedLoginAttempt(now);

        Assert.True(user.IsLockedOut(now));
        Assert.Equal(User.MaxFailedLoginAttempts, user.FailedLoginAttempts);
        Assert.Contains(user.DomainEvents, e => e is UserLockedOutEvent);
    }

    [Fact]
    public void RecordSuccessfulLogin_ResetsFailedAttempts_AndClearsLockout()
    {
        var user = NewUser();
        var now = DateTime.UtcNow;

        for (var i = 0; i < User.MaxFailedLoginAttempts; i++)
            user.RecordFailedLoginAttempt(now);

        Assert.True(user.IsLockedOut(now));

        user.RecordSuccessfulLogin(now);

        Assert.False(user.IsLockedOut(now));
        Assert.Equal(0, user.FailedLoginAttempts);
        Assert.Equal(now, user.LastLoginAtUtc);
    }

    [Fact]
    public void AssignRole_IsIdempotent_AndRaisesEventOnlyOnce()
    {
        var user = NewUser();
        var role = Role.Create("Manager");
        user.ClearDomainEvents();

        user.AssignRole(role);
        user.AssignRole(role);

        Assert.Single(user.Roles);
        Assert.Single(user.DomainEvents.OfType<RoleAssignedToUserEvent>());
    }

    [Fact]
    public void Deactivate_ChangesStatus()
    {
        var user = NewUser();

        user.Deactivate();

        Assert.Equal(UserStatus.Inactive, user.Status);
    }
}
