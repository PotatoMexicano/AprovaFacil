using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Interfaces;

public static class UserInterfaces
{
    public interface IUserService
    {
        Task<UserDTO[]> GetAllUsers(CancellationToken cancellation);
        Task<UserDTO?> GetUser(Int32 idUser, CancellationToken cancellation);
    }

    public interface IUserRepository
    {
        Task<IApplicationUser[]> GetAllUsersAsync(CancellationToken cancellation);
        Task<IApplicationUser?> GetUserAsync(Int32 idUser, CancellationToken cancellation);
    }
}
