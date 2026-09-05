namespace App.Modules.UserManagement.Application.DTOs;

public sealed record RegisterUserRequest(string Email, string Password, string ConfirmPassword, string? DisplayName);
