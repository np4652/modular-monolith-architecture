using App.Modules.UserManagement.Application.DTOs;
using FluentValidation;

namespace App.Modules.UserManagement.Application.Validators;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DisplayName).MaximumLength(128);
        RuleFor(x => x.RoleIds).NotNull();
    }
}
