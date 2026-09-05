using App.Modules.UserManagement.Domain.Exceptions;
using App.Modules.UserManagement.Domain.ValueObjects;

namespace App.UnitTests.Domain.UserManagement;

public class EmailTests
{
    [Theory]
    [InlineData("USER@Example.com", "user@example.com")]
    [InlineData("  someone@example.com  ", "someone@example.com")]
    public void Create_NormalizesToTrimmedLowercase(string input, string expected)
    {
        var email = Email.Create(input);

        Assert.Equal(expected, email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    [InlineData("@missing-local.com")]
    public void Create_Throws_ForInvalidInput(string input)
    {
        Assert.Throws<InvalidEmailException>(() => Email.Create(input));
    }

    [Fact]
    public void Equality_IsValueBased()
    {
        var first = Email.Create("user@example.com");
        var second = Email.Create("USER@example.com");

        Assert.Equal(first, second);
        Assert.True(first == second);
    }
}
