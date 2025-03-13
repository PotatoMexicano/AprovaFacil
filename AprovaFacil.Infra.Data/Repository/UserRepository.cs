using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using AprovaFacil.Infra.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AprovaFacil.Infra.Data.Repository;

public class UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : UserInterfaces.IUserRepository
{
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
}
