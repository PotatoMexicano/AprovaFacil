using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Extensions;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Domain.Results;
using AprovaFacil.Domain.Constants; // Added for ErrorType
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq; // Added for Select

namespace AprovaFacil.Application.Services;

public class UserService(UserInterfaces.IUserRepository repository, 
                       IHttpContextAccessor httpContextAccessor, 
                       ITenantProvider tenantProvider, // Renamed for clarity
                       ITenantRepository tenantRepository) : UserInterfaces.IUserService // Added ITenantRepository
{
    public async Task<Result<UserDTO>> RegisterUser(UserRegisterDTO request, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();
            if (!tenantId.HasValue) 
            {
                return Result<UserDTO>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");
            }

            // Get Tenant to check limits
            Tenant? currentTenant = await tenantRepository.GetByIdAsync(tenantId.Value, cancellation);
            if (currentTenant == null)
            {
                return Result<UserDTO>.Failure(ErrorType.NotFound, "Informações do tenant não encontradas.");
            }

            // Ensure limits are set (might be redundant if constructor always called, but safe)
            currentTenant.SetLimitsBasedOnPlan();

            // Check user limit
            // Note: CurrentUserCount should ideally be updated when users are actually confirmed/created
            // and decremented if a user is disabled/deleted. For simplicity, we check against MaxUsers here.
            // A more robust solution would query the actual number of active users for the tenant.
            // For now, we rely on the CurrentUserCount property of the Tenant object.
            if (currentTenant.CurrentUserCount >= currentTenant.MaxUsers)
            {
                return Result<UserDTO>.Failure(ErrorType.Forbidden, $"Você atingiu o limite de {currentTenant.MaxUsers} usuários para o seu plano.");
            }

            request.TenantId = tenantId.Value;
            IApplicationUser? entity = await repository.RegisterUserAsync(request, cancellation);

            if (entity is null) 
            {
                // It's possible the repository.RegisterUserAsync itself failed due to other reasons (e.g., email exists)
                // The original code returned NotFound, which might be misleading if the user limit was fine but registration failed.
                // Consider if a more specific error from repository.RegisterUserAsync should be propagated.
                return Result<UserDTO>.Failure(ErrorType.Validation, "Falha ao registrar o usuário. Verifique os dados fornecidos.");
            }

            // Increment user count and update tenant
            currentTenant.CurrentUserCount++;
            await tenantRepository.UpdateAsync(currentTenant, cancellation);

            UserDTO result = entity.ToDTO();
            return Result<UserDTO>.Success(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<UserDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao registrar o usuário.");
        }
    }

    public async Task<Result> DisableUser(Int32 idUser, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();
            if (!tenantId.HasValue) return Result.Failure("TenantId não encontrado.");

            ClaimsPrincipal? currentUserClaims = httpContextAccessor.HttpContext?.User;
            if (currentUserClaims == null)
            {
                return Result.Failure("Falha ao obter dados do usuário atual.");
            }

            if (!Int32.TryParse(currentUserClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty, out Int32 idUserAuthenticated))
            {
                return Result.Failure("Falha ao obter dados do usuário atual.");
            }

            if (idUser == idUserAuthenticated)
            {
                return Result.Failure("Você não pode desativar o seu usuário.");
            }

            // Before disabling, get the tenant to decrement user count
            Tenant? currentTenant = await tenantRepository.GetByIdAsync(tenantId.Value, cancellation);
            if (currentTenant == null)
            {
                // This should ideally not happen if tenantId is valid
                Log.Warning($"Tenant não encontrado ({tenantId.Value}) ao tentar desabilitar usuário {idUser}.");
            }

            Boolean result = await repository.DisableUserAsync(idUser, cancellation);
            if (result && currentTenant != null)
            {
                if (currentTenant.CurrentUserCount > 0) // Prevent going below zero
                {
                    currentTenant.CurrentUserCount--;
                    await tenantRepository.UpdateAsync(currentTenant, cancellation);
                }
            }
            return result ? Result.Success() : Result.Failure("Não foi possível desabilitar o usuário.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure("Ocorreu um erro ao desativar o usuário.");
        }
    }

    public async Task<Result> EnableUser(Int32 idUser, CancellationToken cancellation) // Similar logic for enabling if it affects count
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();
            if (!tenantId.HasValue) return Result.Failure("TenantId não encontrado.");

             ClaimsPrincipal? currentUserClaims = httpContextAccessor.HttpContext?.User;
            if (currentUserClaims == null) return Result.Failure("Falha ao obter dados do usuário atual.");
            if (!Int32.TryParse(currentUserClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty, out Int32 idUserAuthenticated))
            {
                return Result.Failure("Falha ao obter dados do usuário atual.");
            }
            if (idUser == idUserAuthenticated) return Result.Failure("Você não pode ativar o seu usuário.");

            // Get Tenant to check limits before enabling, if enabling counts towards the limit again
            Tenant? currentTenant = await tenantRepository.GetByIdAsync(tenantId.Value, cancellation);
            if (currentTenant == null)
            {
                return Result.Failure(ErrorType.NotFound, "Informações do tenant não encontradas para verificar limite de usuários.");
            }
            currentTenant.SetLimitsBasedOnPlan(); // Ensure limits are current

            // Check if enabling this user would exceed the plan limit
            // This assumes that enabling a previously disabled user should count towards the limit.
            // If CurrentUserCount already reflects active users, this check might need adjustment.
            // For now, let's assume enabling increments the count if they were previously not counted.
            IApplicationUser? userToEnable = await repository.GetUserAsync(idUser, tenantId.Value, cancellation);
            if (userToEnable != null && !userToEnable.Active) // Only proceed if user is currently inactive
            {
                 if (currentTenant.CurrentUserCount >= currentTenant.MaxUsers)
                 {
                    return Result.Failure(ErrorType.Forbidden, $"Não é possível ativar o usuário. Limite de {currentTenant.MaxUsers} usuários para o plano atual seria excedido.");
                 }
            }

            Boolean result = await repository.EnableUserAsync(idUser, cancellation);
            if (result && currentTenant != null && userToEnable != null && !userToEnable.Active) // If successfully enabled and was inactive
            {
                currentTenant.CurrentUserCount++;
                await tenantRepository.UpdateAsync(currentTenant, cancellation);
            }
            return result ? Result.Success() : Result.Failure("Não foi possível ativar o usuário.");
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
            Int32? tenantId = tenantProvider.GetTenantId();
            if (!tenantId.HasValue) return Result<UserDTO[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            IApplicationUser[] applicationUsersEntity = await repository.GetAllUsersAsync(tenantId.Value, cancellation);
            UserDTO[] response = [.. applicationUsersEntity.Select(x => UserExtensions.ToDTO(x))];

            if (response is null || !response.Any()) return Result<UserDTO[]>.Failure(ErrorType.NotFound, "Nenhum usuário encontrado.");

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
            Int32? tenantId = tenantProvider.GetTenantId();
            if (!tenantId.HasValue) return Result<UserDTO[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            IApplicationUser[] applicationUsersEntity = await repository.GetAllUsersEnabledAsync(tenantId.Value, cancellation);

            if (applicationUsersEntity is null || !applicationUsersEntity.Any()) return Result<UserDTO[]>.Failure(ErrorType.NotFound, "Nenhum usuário ativo encontrado.");

            UserDTO[] response = [.. applicationUsersEntity.Select(x => UserExtensions.ToDTO(x))];
            return Result<UserDTO[]>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<UserDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar usuários ativos.");
        }
    }

    public async Task<Result<UserDTO>> GetUser(Int32 idUser, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();
            if (!tenantId.HasValue) return Result<UserDTO>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            IApplicationUser? applicationUserEntity = await repository.GetUserAsync(idUser, tenantId.Value, cancellation);
            if (applicationUserEntity is null) return Result<UserDTO>.Failure(ErrorType.NotFound, "Nenhum usuário encontrado.");

            UserDTO response = applicationUserEntity.ToDTO();
            return Result<UserDTO>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<UserDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar o usuário.");
        }
    }

    public async Task<Result<UserDTO>> UpdateUser(UserUpdateDTO request, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId(); // Ensure tenant context for update
            if (!tenantId.HasValue) return Result<UserDTO>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            // It might be relevant to check if the user being updated belongs to the current tenant.
            // The repository.UpdateUserAsync should ideally handle this or be tenant-aware.

            IApplicationUser? applicationUser = await repository.UpdateUserAsync(request, cancellation);
            if (applicationUser is null) return Result<UserDTO>.Failure(ErrorType.NotFound, "Nenhum usuário encontrado para atualizar ou falha na atualização.");

            UserDTO? response = applicationUser?.ToDTO();
            if (response is null) return Result<UserDTO>.Failure(ErrorType.Validation, "Falha ao converter usuário atualizado para DTO.");

            return Result<UserDTO>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<UserDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao atualizar o usuário.");
        }
    }
}

