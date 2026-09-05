using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.ViewModels;

namespace App.Modules.UserManagement.Application.Services;

public interface IRoleService
{
    Task<IReadOnlyList<RoleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<RoleEditViewModel?> GetForEditAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);

    Task UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);
}
