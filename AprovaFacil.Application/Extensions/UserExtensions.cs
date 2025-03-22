using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Application.Extensions;

public class UserExtensions
{
    private readonly Func<User, IApplicationUser> _applicationUserFactory;

    public UserExtensions(Func<User, IApplicationUser> applicationUserFactory)
    {
        this._applicationUserFactory = applicationUserFactory;
    }

    public IApplicationUser ToApplicationUser(User domainUser)
    {
        return _applicationUserFactory(domainUser);
    }

    public static User ToDomainUser(IApplicationUser appUser)
    {
        return new User
        {
            Id = appUser.Id,
            Email = appUser.Email,
            FullName = appUser.FullName,
            Role = appUser.Role,
            Department = appUser.Department,
            PictureUrl = appUser.PictureUrl,
            Enabled = appUser.Enabled,
            Password = null // Password não é mapeado de volta
        };
    }

    public static UserDTO ToDTO(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role switch
            {
                Roles.Requester => "Solicitante",
                Roles.Manager => "Gerente",
                Roles.Director => "Diretor",
                Roles.Assistant => "Assistente",
                Roles.Finance => "Financeiro",
                _ => "Desconhecido"
            },
            Department = user.Department,
            PictureUrl = user.PictureUrl,
            Enabled = user.Enabled,
        };
    }
}
