using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AprovaFacil.Infra.Data.Repository;

public class UserRepository(UserManager<ApplicationUser> userManager) : UserInterfaces.IUserRepository
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
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
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

    public async Task<IApplicationUser[]> GetAllUsersAsync(CancellationToken cancellation)
    {
        ApplicationUser[] entities = await userManager.Users
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
            .ToArrayAsync(cancellation);
        return entities;
    }

    public async Task<IApplicationUser[]> GetAllUsersEnabledAsync(CancellationToken cancellation)
    {
        ApplicationUser[] entities = await userManager.Users
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
            .ToArrayAsync(cancellation);
        return entities;
    }

    public async Task<IApplicationUser?> GetUserAsync(Int32 idUser, CancellationToken cancellation)
    {
        ApplicationUser? entity = await userManager.Users
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
            .Where(x => x.Id == idUser)
            .FirstOrDefaultAsync();
        return entity;
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
            Enabled = true
        };

        IdentityResult result = userManager.CreateAsync(user, request.Password).Result;

        if (result.Succeeded)
        {
            String roleToAssign = user.Role;
            userManager.AddToRoleAsync(user, roleToAssign).Wait();
        }
        else
        {
            // Logar erros, se necessário
            Console.WriteLine($"Erro ao criar {user.Email}: {String.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return user;
    }
}
