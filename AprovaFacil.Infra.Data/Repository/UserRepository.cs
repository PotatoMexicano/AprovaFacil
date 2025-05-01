using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Extensions;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AprovaFacil.Infra.Data.Repository;

public class UserRepository(UserManager<ApplicationUser> userManager, ILogger<UserRepository> logger) : UserInterfaces.IUserRepository
{
    public async Task<Boolean> DisableUserAsync(Int32 idUser, CancellationToken cancellation)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(idUser.ToString());

        if (user == null)
        {
            return false;
        }
        else
        {
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.UtcNow.AddYears(100);
            user.Enabled = false;

            IdentityResult result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public async Task<Boolean> EnableUserAsync(Int32 idUser, CancellationToken cancellation)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(idUser.ToString());

        if (user == null)
        {
            return false;
        }
        else
        {
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            user.Enabled = true;

            IdentityResult result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public async Task<IApplicationUser[]> GetAllUsersAsync(Int32 tenantId, CancellationToken cancellation)
    {
        ApplicationUser[] entities = await userManager.Users
            .Where(u => u.TenantId == tenantId)
            .Select(u => new ApplicationUser
            {
                Id = u.Id,
                FullName = u.FullName,
                Department = u.Department,
                PictureUrl = u.PictureUrl,
                Email = u.Email,
                Enabled = u.Enabled,
                Role = u.Role,
            })
            .OrderByDescending(u => u.Enabled)
            .ThenBy(u => u.FullName)
            .AsNoTracking()
            .ToArrayAsync(cancellation);
        return entities;
    }

    public async Task<IApplicationUser[]> GetAllUsersEnabledAsync(Int32 tenantId, CancellationToken cancellation)
    {
        ApplicationUser[] entities = await userManager.Users
            .Where(u => u.TenantId == tenantId)
            .Select(u => new ApplicationUser
            {
                Id = u.Id,
                FullName = u.FullName,
                Department = u.Department,
                PictureUrl = u.PictureUrl,
                Email = u.Email,
                Enabled = u.Enabled,
                Role = u.Role,
            })
            .Where(u => u.Enabled)
            .OrderBy(u => u.FullName)
            .AsNoTracking()
            .ToArrayAsync(cancellation);
        return entities;
    }

    public async Task<IApplicationUser?> GetUserAsync(Int32 idUser, Int32 tenantId, CancellationToken cancellation)
    {
        ApplicationUser? entity = await userManager.Users
            .Where(u => u.TenantId == tenantId)
            .Select(u => new ApplicationUser
            {
                Id = u.Id,
                UserName = u.UserName,
                FullName = u.FullName,
                Department = u.Department,
                PictureUrl = u.PictureUrl,
                Email = u.Email,
                Enabled = u.Enabled,
                Role = u.Role,
                SecurityStamp = u.SecurityStamp,
            })
            .Where(x => x.Id == idUser)
            .FirstOrDefaultAsync(cancellation);
        return entity;
    }

    public async Task<Dictionary<Int32, IApplicationUser>> GetUsersDictionary(IEnumerable<Int32> usersId, CancellationToken cancellation)
    {
        if (usersId == null)
        {
            return new Dictionary<Int32, IApplicationUser>();
        }

        Dictionary<Int32, IApplicationUser> users = await userManager.Users
            .Select(x => new ApplicationUser
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                Enabled = x.Enabled,
                PictureUrl = x.PictureUrl,
                UserName = x.UserName,
                Department = x.Department,
                Role = x.Role
            })
            .Where(u => usersId.Contains(u.Id))
            .AsNoTracking()
            .ToDictionaryAsync(p => p.Id, q => (IApplicationUser)q, cancellation);

        return users;
    }

    public async Task<IApplicationUser?> RegisterUserAsync(UserRegisterDTO request, CancellationToken cancellation)
    {
        ApplicationUser? existingUser = await userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return null;
        }

        ApplicationUser user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,
            Department = request.Department,
            PictureUrl = request.PictureUrl ?? String.Empty,
            Enabled = true,
            TenantId = request.TenantId,
        };

        IdentityResult result = userManager.CreateAsync(user, request.Password).Result;

        if (result.Succeeded)
        {
            String roleToAssign = user.Role;
            userManager.AddToRoleAsync(user, roleToAssign).Wait(cancellation);
        }
        else
        {
            // Logar erros, se necessário
            Console.WriteLine($"Erro ao criar {user.Email}: {String.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return user;
    }

    public async Task<IApplicationUser?> UpdateUserAsync(UserUpdateDTO request, CancellationToken cancellation)
    {
        try
        {
            IApplicationUser? applicationUser = await userManager.FindByIdAsync(request.Id.ToString());

            if (applicationUser is null)
            {
                return null;
            }

            UserExtensions.Update(ref applicationUser, request);

            if (!String.IsNullOrEmpty(request.Password))
            {
                String token = await userManager.GeneratePasswordResetTokenAsync((ApplicationUser)applicationUser);
                IdentityResult result = await userManager.ResetPasswordAsync((ApplicationUser)applicationUser, token, request.Password);
            }

            IdentityResult updateResult = await userManager.UpdateAsync((ApplicationUser)applicationUser);

            if (!updateResult.Succeeded)
            {
                return null;
            }

            if (request.Role is not null && request.Role.Any())
            {
                IList<String> currentRoles = await userManager.GetRolesAsync((ApplicationUser)applicationUser);

                await userManager.RemoveFromRolesAsync((ApplicationUser)applicationUser, currentRoles);

                await userManager.AddToRoleAsync((ApplicationUser)applicationUser, request.Role);
            }

            return applicationUser;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao atualizar usuário.");
            return null;
        }
    }
}
