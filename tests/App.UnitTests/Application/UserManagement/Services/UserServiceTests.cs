using App.BuildingBlocks.Application.Abstractions;
using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.Services;
using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.Exceptions;
using App.Modules.UserManagement.Domain.Repositories;
using App.Modules.UserManagement.Domain.ValueObjects;
using Moq;

namespace App.UnitTests.Application.UserManagement.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IRoleRepository> _roleRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private UserService CreateSut() =>
        new(_userRepository.Object, _roleRepository.Object, _passwordHasher.Object, _unitOfWork.Object);

    [Fact]
    public async Task RegisterAsync_Throws_WhenEmailAlreadyExists()
    {
        _userRepository
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = CreateSut();
        var request = new RegisterUserRequest("taken@example.com", "Password123!", "Password123!", null);

        await Assert.ThrowsAsync<EmailAlreadyInUseException>(() => sut.RegisterAsync(request));

        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_HashesPassword_AndPersists_WhenEmailIsAvailable()
    {
        _userRepository
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasher.Setup(h => h.Hash("Password123!")).Returns("hashed-password");

        var sut = CreateSut();
        var request = new RegisterUserRequest("new@example.com", "Password123!", "Password123!", "New User");

        var userId = await sut.RegisterAsync(request);

        Assert.NotEqual(Guid.Empty, userId);
        _userRepository.Verify(r => r.AddAsync(
            It.Is<User>(u => u.Email.Value == "new@example.com" && u.PasswordHash.Value == "hashed-password"),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
