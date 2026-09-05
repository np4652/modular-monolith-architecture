using App.BuildingBlocks.Application.Abstractions;
using App.BuildingBlocks.Domain.Exceptions;
using App.BuildingBlocks.Infrastructure.Caching;
using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.ViewModels;
using App.Modules.UserManagement.Domain.Entities;
using App.Modules.UserManagement.Domain.Exceptions;
using App.Modules.UserManagement.Domain.Repositories;

namespace App.Modules.UserManagement.Application.Services;

public sealed class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<RoleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cacheService.GetOrCreateAsync(
            UserManagementCacheKeys.AllRoles,
            CacheExpiration.Medium,
            async ct =>
            {
                var roles = await _roleRepository.GetAllAsync(ct);
                return (IReadOnlyList<RoleListItemViewModel>)roles
                    .Select(r => new RoleListItemViewModel(r.Id, r.Name, r.Description, r.Permissions.Count, r.IsSystemRole))
                    .ToList();
            },
            cancellationToken);
    }

    public async Task<RoleEditViewModel?> GetForEditAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role is null) return null;

        var grantedCodes = role.Permissions.Select(p => p.PermissionCode).ToHashSet();

        var permissions = UserManagementPermissions.All
            .Select(p => new PermissionOptionViewModel(p.Code, p.Category, p.DisplayName, grantedCodes.Contains(p.Code)))
            .ToList();

        return new RoleEditViewModel(role.Id, role.Name, role.Description, role.IsSystemRole, permissions);
    }

    public async Task<Guid> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (await _roleRepository.ExistsByNameAsync(request.Name, cancellationToken))
            throw new DuplicateRoleNameException(request.Name);

        var role = Role.Create(request.Name, request.Description);
        role.ReplacePermissions(request.PermissionCodes);

        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync(UserManagementCacheKeys.AllRoles, cancellationToken);

        return role.Id;
    }

    public async Task UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken)
                   ?? throw new NotFoundException(nameof(Role), request.RoleId);

        if (role.IsSystemRole)
            throw new SystemRoleImmutableException(role.Name);

        var existing = await _roleRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null && existing.Id != role.Id)
            throw new DuplicateRoleNameException(request.Name);

        role.Rename(request.Name);
        role.UpdateDescription(request.Description);
        role.ReplacePermissions(request.PermissionCodes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync(UserManagementCacheKeys.AllRoles, cancellationToken);
    }

    public async Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken)
                   ?? throw new NotFoundException(nameof(Role), roleId);

        if (role.IsSystemRole)
            throw new SystemRoleImmutableException(role.Name);

        _roleRepository.Remove(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync(UserManagementCacheKeys.AllRoles, cancellationToken);
    }
}
