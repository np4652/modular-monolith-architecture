using App.BuildingBlocks.Application.Abstractions;
using App.Modules.UserManagement.Application.Pipelines.Authentication;
using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.Enums;
using App.Modules.UserManagement.Domain.Exceptions;
using App.Modules.UserManagement.Domain.Repositories;
using App.Modules.UserManagement.Domain.ValueObjects;
using Moq;

namespace App.UnitTests.Application.UserManagement.Pipelines;

public class AuthenticationPipelineStepsTests
{
    private static User ActiveUser() =>
        User.Register(Email.Create("user@example.com"), PasswordHash.FromHash("stored-hash"));

    private static Task NoOpNext() => Task.CompletedTask;

    public class CheckAccountLockoutStepTests
    {
        [Fact]
        public async Task Throws_InvalidCredentials_WhenUserDoesNotExist()
        {
            var repository = new Mock<IUserRepository>();
            repository
                .Setup(r => r.GetByEmailWithRolesAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var step = new CheckAccountLockoutStep(repository.Object);
            var context = new AuthenticationPipelineContext("missing@example.com", "irrelevant", DateTime.UtcNow);

            await Assert.ThrowsAsync<InvalidCredentialsException>(() => step.HandleAsync(context, NoOpNext, default));
        }

        [Fact]
        public async Task Throws_AccountLocked_WhenUserIsCurrentlyLockedOut()
        {
            var user = ActiveUser();
            var now = DateTime.UtcNow;
            for (var i = 0; i < User.MaxFailedLoginAttempts; i++)
                user.RecordFailedLoginAttempt(now);

            var repository = new Mock<IUserRepository>();
            repository
                .Setup(r => r.GetByEmailWithRolesAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var step = new CheckAccountLockoutStep(repository.Object);
            var context = new AuthenticationPipelineContext("user@example.com", "irrelevant", now);

            await Assert.ThrowsAsync<AccountLockedException>(() => step.HandleAsync(context, NoOpNext, default));
        }

        [Fact]
        public async Task PopulatesContextUser_AndContinues_WhenNotLocked()
        {
            var user = ActiveUser();
            var repository = new Mock<IUserRepository>();
            repository
                .Setup(r => r.GetByEmailWithRolesAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var step = new CheckAccountLockoutStep(repository.Object);
            var context = new AuthenticationPipelineContext("user@example.com", "irrelevant", DateTime.UtcNow);

            var nextCalled = false;
            await step.HandleAsync(context, () => { nextCalled = true; return Task.CompletedTask; }, default);

            Assert.True(nextCalled);
            Assert.Same(user, context.User);
        }
    }

    public class ValidateCredentialsStepTests
    {
        [Fact]
        public async Task Throws_AndRecordsFailedAttempt_WhenPasswordIsWrong()
        {
            var user = ActiveUser();
            var hasher = new Mock<IPasswordHasher>();
            hasher.Setup(h => h.Verify("wrong-password", user.PasswordHash.Value)).Returns(false);

            var step = new ValidateCredentialsStep(hasher.Object);
            var context = new AuthenticationPipelineContext("user@example.com", "wrong-password", DateTime.UtcNow)
            {
                User = user,
            };

            await Assert.ThrowsAsync<InvalidCredentialsException>(() => step.HandleAsync(context, NoOpNext, default));
            Assert.Equal(1, user.FailedLoginAttempts);
        }

        [Fact]
        public async Task Continues_WhenPasswordIsCorrect()
        {
            var user = ActiveUser();
            var hasher = new Mock<IPasswordHasher>();
            hasher.Setup(h => h.Verify("correct-password", user.PasswordHash.Value)).Returns(true);

            var step = new ValidateCredentialsStep(hasher.Object);
            var context = new AuthenticationPipelineContext("user@example.com", "correct-password", DateTime.UtcNow)
            {
                User = user,
            };

            var nextCalled = false;
            await step.HandleAsync(context, () => { nextCalled = true; return Task.CompletedTask; }, default);

            Assert.True(nextCalled);
        }
    }

    public class CheckAccountStatusStepTests
    {
        [Fact]
        public async Task Throws_WhenAccountIsNotActive()
        {
            var user = ActiveUser();
            user.Deactivate();

            var step = new CheckAccountStatusStep();
            var context = new AuthenticationPipelineContext("user@example.com", "irrelevant", DateTime.UtcNow)
            {
                User = user,
            };

            await Assert.ThrowsAsync<AccountNotActiveException>(() => step.HandleAsync(context, NoOpNext, default));
        }

        [Fact]
        public async Task Continues_WhenAccountIsActive()
        {
            var user = ActiveUser();
            var step = new CheckAccountStatusStep();
            var context = new AuthenticationPipelineContext("user@example.com", "irrelevant", DateTime.UtcNow)
            {
                User = user,
            };

            var nextCalled = false;
            await step.HandleAsync(context, () => { nextCalled = true; return Task.CompletedTask; }, default);

            Assert.True(nextCalled);
        }
    }

    public class GenerateAuthenticationResultStepTests
    {
        [Fact]
        public async Task ProducesDistinctPermissions_AcrossAllAssignedRoles()
        {
            var user = ActiveUser();
            var roleA = Role.Create("A");
            roleA.GrantPermission("users.view");
            var roleB = Role.Create("B");
            roleB.GrantPermission("users.view");
            roleB.GrantPermission("roles.view");
            user.AssignRole(roleA);
            user.AssignRole(roleB);

            var step = new GenerateAuthenticationResultStep();
            var context = new AuthenticationPipelineContext("user@example.com", "irrelevant", DateTime.UtcNow)
            {
                User = user,
            };

            await step.HandleAsync(context, NoOpNext, default);

            Assert.NotNull(context.Result);
            Assert.Equal(2, context.Result!.Permissions.Count);
            Assert.Contains("users.view", context.Result.Permissions);
            Assert.Contains("roles.view", context.Result.Permissions);
        }
    }
}
