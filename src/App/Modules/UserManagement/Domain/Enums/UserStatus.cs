namespace App.Modules.UserManagement.Domain.Enums;

/// <summary>
/// Administrator-controlled account status. Temporary lockout after repeated failed
/// logins is tracked separately on <see cref="Entities.User"/> - it is not a status.
/// </summary>
public enum UserStatus
{
    Active = 1,
    Inactive = 2,
}
