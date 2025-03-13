using AprovaFacil.Application.Extensions;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Application.Services;

public class UserService(UserInterfaces.IUserRepository repository) : UserInterfaces.IUserService
{
    public async Task<UserDTO[]> GetAllUsers(CancellationToken cancellation)
    {
        IApplicationUser[] applicationUsersEntity = await repository.GetAllUsersAsync(cancellation);
        User[] userEntities = applicationUsersEntity.Select(UserExtensions.ToDomainUser).ToArray();
        UserDTO[] dtos = userEntities.Select(UserExtensions.ToDTO).ToArray();
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
}
