using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Extensions;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Domain.Results;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Security.Claims;

namespace AprovaFacil.Application.Services;

public class UserService(UserInterfaces.IUserRepository repository, IHttpContextAccessor httpContextAccessor) : UserInterfaces.IUserService
{
    public async Task<Result> DisableUser(Int32 idUser, CancellationToken cancellation)
    {
        try
        {
            ClaimsPrincipal? currentUser = httpContextAccessor.HttpContext?.User;

            if (currentUser == null)
            {
                return Result.Failure("Falha ao obter dados do usuário atual.");
            }

            if (!Int32.TryParse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty, out Int32 idUserAuthenticated))
            {
                return Result.Failure("Falha ao obter dados do usuário atual.");
            }

            if (idUser == idUserAuthenticated)
            {
                return Result.Failure("Você não pode desativar o seu usuário.");
            }

            Boolean result = await repository.DisableUserAsync(idUser, cancellation);

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure("Ocorreu um erro ao desativar o usuário.");
        }
    }

    public async Task<Result> EnableUser(Int32 idUser, CancellationToken cancellation)
    {
        try
        {
            ClaimsPrincipal? currentUser = httpContextAccessor.HttpContext?.User;

            if (currentUser == null)
            {
                return Result.Failure("Falha ao obter dados do usuário atual.");
            }

            if (!Int32.TryParse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty, out Int32 idUserAuthenticated))
            {
                return Result.Failure("Falha ao obter dados do usuário atual.");
            }

            if (idUser == idUserAuthenticated)
            {
                return Result.Failure("Você não pode ativar o seu usuário.");
            }

            Boolean result = await repository.EnableUserAsync(idUser, cancellation);

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure("Ocorreu um erro ao ativar o usuário.");
        }
    }

    public async Task<Result<UserDTO[]>> GetAllUsers(CancellationToken cancellation)
    {
        try
        {
            IApplicationUser[] applicationUsersEntity = await repository.GetAllUsersAsync(cancellation);
            UserDTO[] response = [.. applicationUsersEntity.Select(x => UserExtensions.ToDTO(x))];

            if (response is null) return Result<UserDTO[]>.Failure(ErrorType.NotFound, "Nenhum usuário encontrado.");

            return Result<UserDTO[]>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<UserDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar os usuários.");
        }
    }

    public async Task<Result<UserDTO[]>> GetAllusersEnabled(CancellationToken cancellation)
    {
        try
        {
            IApplicationUser[] applicationUsersEntity = await repository.GetAllUsersEnabledAsync(cancellation);

            if (applicationUsersEntity is null) return Result<UserDTO[]>.Failure(ErrorType.NotFound, "Nenhum usuário encontrado.");

            UserDTO[] response = [.. applicationUsersEntity.Select(x => UserExtensions.ToDTO(x))];

            return Result<UserDTO[]>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<UserDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar usuários.");
        }
    }

    public async Task<Result<UserDTO>> GetUser(Int32 idUser, CancellationToken cancellation)
    {
        try
        {
            IApplicationUser? applicationUserEntity = await repository.GetUserAsync(idUser, cancellation);

            if (applicationUserEntity is null) return Result<UserDTO>.Failure(ErrorType.NotFound, "Nenhum usuário encontrado.");

            UserDTO response = applicationUserEntity.ToDTO();

            return Result<UserDTO>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<UserDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar usuários.");
        }
    }

    public async Task<Result<UserDTO>> RegisterUser(UserRegisterDTO request, CancellationToken cancellation)
    {
        try
        {
            IApplicationUser? entity = await repository.RegisterUserAsync(request, cancellation);

            if (entity is null) return Result<UserDTO>.Failure(ErrorType.NotFound, "Nenhum usuário encontrado.");

            UserDTO result = entity.ToDTO();

            return Result<UserDTO>.Success(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<UserDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao registrar o usuário.");
        }
    }

    public async Task<Result<UserDTO>> UpdateUser(UserUpdateDTO request, CancellationToken cancellation)
    {
        try
        {
            IApplicationUser? applicationUser = await repository.UpdateUserAsync(request, cancellation);

            if (applicationUser is null) return Result<UserDTO>.Failure(ErrorType.NotFound, "Nenhum usuário encontrado.");

            UserDTO? response = applicationUser?.ToDTO();

            if (response is null) return Result<UserDTO>.Failure(ErrorType.Validation, "Falha ao validar usuário.");

            return Result<UserDTO>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<UserDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao atualizar o usuário.");
        }
    }
}
