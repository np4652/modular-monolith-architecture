namespace App.Modules.UserManagement.Presentation;

/// <summary>
/// Named page routes for this module, so PageModels and components redirect/link by
/// constant rather than by retyping a literal path.
/// </summary>
public static class ModuleRoute
{
    public const string Login = "/Account/Login";
    public const string AccessDenied = "/Account/AccessDenied";
    public const string UsersIndex = "/Users/Index";
    public const string RolesIndex = "/Roles/Index";
}
