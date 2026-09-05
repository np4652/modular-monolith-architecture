using App.Modules.UserManagement.Application.DTOs;
using FluentValidation;

namespace App.Modules.UserManagement.Application.Validators;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.DisplayName).MaximumLength(128);
        RuleFor(x => x.RoleIds).NotNull();
    }
}
