using App.BuildingBlocks.Application.Abstractions;

namespace App.BuildingBlocks.Infrastructure.Security;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plainTextPassword) => BCrypt.Net.BCrypt.EnhancedHashPassword(plainTextPassword, 12);

    public bool Verify(string plainTextPassword, string hash) =>
        BCrypt.Net.BCrypt.EnhancedVerify(plainTextPassword, hash);
}
