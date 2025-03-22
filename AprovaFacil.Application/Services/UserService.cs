using AprovaFacil.Application.Extensions;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AprovaFacil.Application.Services;

public class UserService(UserInterfaces.IUserRepository repository, IHttpContextAccessor httpContextAccessor, Func<User, IApplicationUser> userFactory) : UserInterfaces.IUserService
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
        User[] userEntities = [.. applicationUsersEntity.Select(UserExtensions.ToDomainUser)];
        UserDTO[] dtos = [.. userEntities.Select(UserExtensions.ToDTO)];
        return dtos;
    }

    public async Task<UserDTO[]> GetAllusersEnabled(CancellationToken cancellation)
    {
        IApplicationUser[] applicationUsersEntity = await repository.GetAllUsersEnabledAsync(cancellation);
        User[] userEntities = [.. applicationUsersEntity.Select(UserExtensions.ToDomainUser)];
        UserDTO[] dtos = [.. userEntities.Select(UserExtensions.ToDTO)];
        return dtos;
    }

    public async Task<UserDTO?> GetUser(Int32 idUser, CancellationToken cancellation)
    {
        IApplicationUser? applicationUserEntity = await repository.GetUserAsync(idUser, cancellation);
        if (applicationUserEntity is null) return null;
        User userEntity = UserExtensions.ToDomainUser(applicationUserEntity);
        UserDTO dto = UserExtensions.ToDTO(userEntity);
        return dto;
    }

    public async Task<UserDTO?> RegisterUser(UserRegisterDTO request, CancellationToken cancellation)
    {

        IApplicationUser? entity = await repository.RegisterUserAsync(request, cancellation);

        if (entity is null)
        {
            return null;
        }

        return UserExtensions.ToDTO(UserExtensions.ToDomainUser(entity));
    }
}
