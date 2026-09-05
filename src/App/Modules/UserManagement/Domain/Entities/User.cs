using App.BuildingBlocks.Domain.Abstractions;
using App.BuildingBlocks.Domain.Primitives;
using App.Modules.UserManagement.Domain.Enums;
using App.Modules.UserManagement.Domain.Events;
using App.Modules.UserManagement.Domain.Exceptions;
using App.Modules.UserManagement.Domain.ValueObjects;

namespace App.Modules.UserManagement.Domain.Entities;

public sealed class User : AggregateRoot<Guid>, IAuditableEntity, ISoftDeletable
{
    public const int MaxFailedLoginAttempts = 5;
    public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private readonly List<Role> _roles = [];

    private User()
    {
    }

    private User(Email email, PasswordHash passwordHash) : base(Guid.NewGuid())
    {
        Email = email;
        PasswordHash = passwordHash;
        Status = UserStatus.Active;
    }

    public Email Email { get; private set; } = null!;

    public PasswordHash PasswordHash { get; private set; } = null!;

    public string? DisplayName { get; private set; }

    public UserStatus Status { get; private set; }

    public int FailedLoginAttempts { get; private set; }

    public DateTime? LockedUntilUtc { get; private set; }

    public DateTime? LastLoginAtUtc { get; private set; }

    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public DateTime CreatedAtUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTime? ModifiedAtUtc { get; private set; }

    public string? ModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }

    public bool IsLockedOut(DateTime asOfUtc) => LockedUntilUtc is not null && LockedUntilUtc > asOfUtc;

    public static User Register(Email email, PasswordHash passwordHash, string? displayName = null)
    {
        var user = new User(email, passwordHash) { DisplayName = displayName };
        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, email.Value));
        return user;
    }

    public void ChangePassword(PasswordHash newPasswordHash) => PasswordHash = newPasswordHash;

    public void UpdateProfile(string? displayName) => DisplayName = displayName;

    public void Activate() => Status = UserStatus.Active;

    public void Deactivate() => Status = UserStatus.Inactive;

    public void RecordFailedLoginAttempt(DateTime occurredOnUtc)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts < MaxFailedLoginAttempts)
            return;

        LockedUntilUtc = occurredOnUtc.Add(LockoutDuration);
        RaiseDomainEvent(new UserLockedOutEvent(Id, LockedUntilUtc.Value));
    }

    public void RecordSuccessfulLogin(DateTime occurredOnUtc)
    {
        FailedLoginAttempts = 0;
        LockedUntilUtc = null;
        LastLoginAtUtc = occurredOnUtc;
        RaiseDomainEvent(new UserLoggedInEvent(Id));
    }

    public void AssignRole(Role role)
    {
        if (_roles.Any(r => r.Id == role.Id))
            return;

        _roles.Add(role);
        RaiseDomainEvent(new RoleAssignedToUserEvent(Id, role.Id));
    }

    public void RemoveRole(Guid roleId) => _roles.RemoveAll(r => r.Id == roleId);

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

    public void MarkDeleted(DateTime occurredOnUtc)
    {
        IsDeleted = true;
        DeletedAtUtc = occurredOnUtc;
    }
}
