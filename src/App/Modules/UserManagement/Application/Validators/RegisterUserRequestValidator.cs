using App.Modules.UserManagement.Application.DTOs;
using FluentValidation;

namespace App.Modules.UserManagement.Application.Validators;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
        RuleFor(x => x.DisplayName).MaximumLength(128);
    }
}
