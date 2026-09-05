using App.Modules.UserManagement.Application.DTOs;
using FluentValidation;

namespace App.Modules.UserManagement.Application.Validators;

public sealed class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Description).MaximumLength(256);
        RuleFor(x => x.PermissionCodes).NotNull();
        RuleForEach(x => x.PermissionCodes)
            .Must(UserManagementPermissions.All.Select(p => p.Code).Contains)
            .WithMessage("'{PropertyValue}' is not a recognized permission.");
    }
}
