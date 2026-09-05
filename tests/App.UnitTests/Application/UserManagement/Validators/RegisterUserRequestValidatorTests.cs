using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.Validators;
using FluentValidation.TestHelper;

namespace App.UnitTests.Application.UserManagement.Validators;

public class RegisterUserRequestValidatorTests
{
    private readonly RegisterUserRequestValidator _validator = new();

    [Fact]
    public void Fails_WhenEmailIsInvalid()
    {
        var request = new RegisterUserRequest("not-an-email", "Password123!", "Password123!", null);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Fails_WhenPasswordsDoNotMatch()
    {
        var request = new RegisterUserRequest("user@example.com", "Password123!", "Different123!", null);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void Fails_WhenPasswordTooShort()
    {
        var request = new RegisterUserRequest("user@example.com", "short", "short", null);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Succeeds_ForValidRequest()
    {
        var request = new RegisterUserRequest("user@example.com", "Password123!", "Password123!", "Jane Doe");

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
