namespace App.BuildingBlocks.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string plainTextPassword);

    bool Verify(string plainTextPassword, string hash);
}
