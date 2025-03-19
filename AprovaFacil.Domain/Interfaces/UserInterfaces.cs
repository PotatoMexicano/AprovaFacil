using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Interfaces;

public static class UserInterfaces
{
    public interface IUserService
    {
        Task<UserDTO[]> GetAllUsers(CancellationToken cancellation);
        Task<UserDTO[]> GetAllusersEnabled(CancellationToken cancellation);
        Task<UserDTO?> GetUser(Int32 idUser, CancellationToken cancellation);

        Task<UserDTO?> RegisterUser(UserRegisterDTO request, CancellationToken cancellation);

        Task<Boolean> DisableUser(Int32 idUser, CancellationToken cancellation);
        Task<Boolean> EnableUser(Int32 idUser, CancellationToken cancellation);
    }

    public interface IUserRepository
    {
        Task<IApplicationUser[]> GetAllUsersAsync(CancellationToken cancellation);
        Task<IApplicationUser[]> GetAllUsersEnabledAsync(CancellationToken cancellation);
        Task<IApplicationUser?> GetUserAsync(Int32 idUser, CancellationToken cancellation);

        Task<IApplicationUser?> RegisterUserAsync(UserRegisterDTO request, CancellationToken cancellation);

        Task<Boolean> DisableUserAsync(Int32 idUser, CancellationToken cancellation);
        Task<Boolean> EnableUserAsync(Int32 idUser, CancellationToken cancellation);
    }
}
