using App.BuildingBlocks.Application.Common;
using App.Modules.UserManagement.Application.DTOs;
using App.Modules.UserManagement.Application.ViewModels;

namespace App.Modules.UserManagement.Application.Services;

public interface IUserService
{
    Task<Guid> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);

    Task ActivateAsync(Guid userId, CancellationToken cancellationToken = default);

    Task DeactivateAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserListViewModel> SearchAsync(string? searchTerm, PaginationRequest pagination, CancellationToken cancellationToken = default);

    Task<UserDetailsViewModel?> GetDetailsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserEditViewModel?> GetForEditAsync(Guid userId, CancellationToken cancellationToken = default);
}
