using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Application.Common;
using App.BuildingBlocks.Domain.Exceptions;
using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.ViewModels;
using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.Enums;
using App.Modules.UserManagement.Domain.Exceptions;
using App.Modules.UserManagement.Domain.Repositories;
using App.Modules.UserManagement.Domain.ValueObjects;

namespace App.Modules.UserManagement.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(request.Email);

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new EmailAlreadyInUseException(email.Value);

        var passwordHash = PasswordHash.FromHash(_passwordHasher.Hash(request.Password));
        var user = User.Register(email, passwordHash, request.DisplayName);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(request.Email);

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new EmailAlreadyInUseException(email.Value);

        var passwordHash = PasswordHash.FromHash(_passwordHasher.Hash(request.Password));
        var user = User.Register(email, passwordHash, request.DisplayName);

        await AssignRolesAsync(user, request.RoleIds, cancellationToken);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(request.UserId, cancellationToken)
                   ?? throw new NotFoundException(nameof(User), request.UserId);

        user.UpdateProfile(request.DisplayName);

        if (request.IsActive) user.Activate();
        else user.Deactivate();

        foreach (var roleId in user.Roles.Select(r => r.Id).Except(request.RoleIds).ToList())
            user.RemoveRole(roleId);

        await AssignRolesAsync(user, request.RoleIds.Except(user.Roles.Select(r => r.Id)).ToList(), cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                   ?? throw new NotFoundException(nameof(User), userId);

        user.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                   ?? throw new NotFoundException(nameof(User), userId);

        user.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserListViewModel> SearchAsync(
        string? searchTerm,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        var (users, totalCount) = await _userRepository.SearchAsync(
            searchTerm, pagination.Skip, pagination.PageSize, cancellationToken);

        var items = users
            .Select(u => new UserListItemViewModel(
                u.Id,
                u.Email.Value,
                u.DisplayName,
                u.Status.ToString(),
                u.CreatedAtUtc,
                u.Roles.Select(r => r.Name).ToList()))
            .ToList();

        return new UserListViewModel(
            new PagedResult<UserListItemViewModel>(items, totalCount, pagination.Page, pagination.PageSize),
            searchTerm);
    }

    public async Task<UserDetailsViewModel?> GetDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(userId, cancellationToken);
        if (user is null) return null;

        return new UserDetailsViewModel(
            user.Id,
            user.Email.Value,
            user.DisplayName,
            user.Status.ToString(),
            user.LastLoginAtUtc,
            user.CreatedAtUtc,
            user.Roles.Select(r => new RoleSummaryViewModel(r.Id, r.Name)).ToList());
    }

    public async Task<UserEditViewModel?> GetForEditAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(userId, cancellationToken);
        if (user is null) return null;

        var allRoles = await _roleRepository.GetAllAsync(cancellationToken);
        var assignedRoleIds = user.Roles.Select(r => r.Id).ToHashSet();

        return new UserEditViewModel(
            user.Id,
            user.Email.Value,
            user.DisplayName,
            user.Status == UserStatus.Active,
            allRoles.Select(r => new RoleOptionViewModel(r.Id, r.Name, assignedRoleIds.Contains(r.Id))).ToList());
    }

    private async Task AssignRolesAsync(User user, IReadOnlyList<Guid> roleIds, CancellationToken cancellationToken)
    {
        foreach (var roleId in roleIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken)
                       ?? throw new NotFoundException(nameof(Role), roleId);

            user.AssignRole(role);
        }
    }
}
