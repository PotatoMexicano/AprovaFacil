using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;
using AprovaFacil.Domain.Results;

namespace AprovaFacil.Domain.Interfaces;

public static class UserInterfaces
{
    public interface IUserService
    {
        Task<Result<UserDTO[]>> GetAllUsers(CancellationToken cancellation);
        Task<Result<UserDTO[]>> GetAllusersEnabled(CancellationToken cancellation);
        Task<Result<UserDTO>> GetUser(Int32 idUser, CancellationToken cancellation);

        Task<Result<UserDTO>> RegisterUser(UserRegisterDTO request, CancellationToken cancellation);
        Task<Result<UserDTO>> UpdateUser(UserUpdateDTO request, CancellationToken cancellation);

        Task<Result> DisableUser(Int32 idUser, CancellationToken cancellation);
        Task<Result> EnableUser(Int32 idUser, CancellationToken cancellation);

    }

    public interface IUserRepository
    {
        Task<Dictionary<Int32, IApplicationUser>> GetUsersDictionary(IEnumerable<Int32> usersId, CancellationToken cancellation);

        Task<IApplicationUser[]> GetAllUsersAsync(Int32 tenantId, CancellationToken cancellation);
        Task<IApplicationUser[]> GetAllUsersEnabledAsync(Int32 tenantId, CancellationToken cancellation);
        Task<IApplicationUser?> GetUserAsync(Int32 idUser, Int32 tenantId, CancellationToken cancellation);

        Task<IApplicationUser?> RegisterUserAsync(UserRegisterDTO request, CancellationToken cancellation);
        Task<IApplicationUser?> UpdateUserAsync(UserUpdateDTO user, CancellationToken cancellation);

        Task<Boolean> DisableUserAsync(Int32 idUser, CancellationToken cancellation);
        Task<Boolean> EnableUserAsync(Int32 idUser, CancellationToken cancellation);

    }
}
