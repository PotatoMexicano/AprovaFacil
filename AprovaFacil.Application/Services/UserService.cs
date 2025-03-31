using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Extensions;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AprovaFacil.Application.Services;

public class UserService(UserInterfaces.IUserRepository repository, IHttpContextAccessor httpContextAccessor) : UserInterfaces.IUserService
{
    public async Task<Boolean> DisableUser(Int32 idUser, CancellationToken cancellation)
    {
        ClaimsPrincipal? currentUser = httpContextAccessor.HttpContext?.User;

        if (currentUser == null)
        {
            return false;
        }

        if (!Int32.TryParse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty, out Int32 idUserAuthenticated))
        {
            return false;
        }

        if (idUser == idUserAuthenticated)
        {
            return false;
        }

        Boolean result = await repository.DisableUserAsync(idUser, cancellation);
        return result;
    }

    public async Task<Boolean> EnableUser(Int32 idUser, CancellationToken cancellation)
    {
        ClaimsPrincipal? currentUser = httpContextAccessor.HttpContext?.User;

        if (currentUser == null)
        {
            return false;
        }

        if (!Int32.TryParse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty, out Int32 idUserAuthenticated))
        {
            return false;
        }

        if (idUser == idUserAuthenticated)
        {
            return false;
        }

        Boolean result = await repository.EnableUserAsync(idUser, cancellation);
        return result;
    }

    public async Task<UserDTO[]> GetAllUsers(CancellationToken cancellation)
    {
        IApplicationUser[] applicationUsersEntity = await repository.GetAllUsersAsync(cancellation);
        UserDTO[] response = [.. applicationUsersEntity.Select(x => UserExtensions.ToDTO(x))];
        return response;
    }

    public async Task<UserDTO[]> GetAllusersEnabled(CancellationToken cancellation)
    {
        IApplicationUser[] applicationUsersEntity = await repository.GetAllUsersEnabledAsync(cancellation);
        UserDTO[] response = [.. applicationUsersEntity.Select(x => UserExtensions.ToDTO(x))];
        return response;
    }

    public async Task<UserDTO?> GetUser(Int32 idUser, CancellationToken cancellation)
    {
        IApplicationUser? applicationUserEntity = await repository.GetUserAsync(idUser, cancellation);
        if (applicationUserEntity is null) return null;
        UserDTO response = applicationUserEntity.ToDTO();
        return response;
    }

    public async Task<UserDTO?> RegisterUser(UserRegisterDTO request, CancellationToken cancellation)
    {

        IApplicationUser? entity = await repository.RegisterUserAsync(request, cancellation);

        if (entity is null)
        {
            return null;
        }

        return entity.ToDTO();
    }

    public async Task<UserDTO?> UpdateUser(UserUpdateDTO request, CancellationToken cancellation)
    {
        IApplicationUser? applicationUser = await repository.UpdateUserAsync(request, cancellation);
        return applicationUser?.ToDTO();
    }
}
