using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Extensions;

public static class UserExtensions
{
    public static UserDTO ToDTO(this IApplicationUser appUser, IEnumerable<String>? roles = default)
    {
        return new UserDTO
        {
            Id = appUser.Id,
            Email = appUser.Email,
            FullName = appUser.FullName,
            Role = appUser.Role,
            IdentityRoles = roles?.ToList() ?? [],
            RoleLabel = appUser.Role switch
            {
                Roles.Requester => "Requisitante",
                Roles.Manager => "Gerente",
                Roles.Director => "Diretor",
                Roles.Assistant => "Assistente",
                Roles.Finance => "Financeiro",
                _ => "Desconhecido"
            },
            Department = appUser.Department,
            DepartmentLabel = appUser.Department switch
            {
                Departaments.Engineer => "Engenharia",
                Departaments.Sales => "Vendas",
                Departaments.Operations => "Operações",
                Departaments.Marketing => "Marketing",
                Departaments.IT => "Tecnologia da Informação",
                Departaments.HR => "Recursos Humanos",
                Departaments.Finance => "Financeiro",
                _ => "Desconhecido"
            },
            PictureUrl = appUser.PictureUrl,
            Enabled = appUser.Enabled,
        };
    }

    public static UserDTO ToDTO(this RequestDirector appUser, IEnumerable<String>? roles = default)
    {
        return new UserDTO
        {
            Id = appUser.User.Id,
            Email = appUser.User.Email,
            FullName = appUser.User.FullName,
            Role = appUser.User.Role,
            IdentityRoles = roles?.ToList() ?? [],
            RoleLabel = appUser.User.Role switch
            {
                Roles.Requester => "Requisitante",
                Roles.Manager => "Gerente",
                Roles.Director => "Diretor",
                Roles.Assistant => "Assistente",
                Roles.Finance => "Financeiro",
                _ => "Desconhecido"
            },
            Department = appUser.User.Department,
            DepartmentLabel = appUser.User.Department switch
            {
                Departaments.Engineer => "Engenharia",
                Departaments.Sales => "Vendas",
                Departaments.Operations => "Operações",
                Departaments.Marketing => "Marketing",
                Departaments.IT => "Tecnologia da Informação",
                Departaments.HR => "Recursos Humanos",
                Departaments.Finance => "Financeiro",
                _ => "Desconhecido"
            },
            PictureUrl = appUser.User.PictureUrl,
            Enabled = appUser.User.Enabled,
            RequestApproved = appUser.Approved,
        };
    }

    public static UserDTO ToDTO(this RequestManager appUser, IEnumerable<String>? roles = default)
    {
        return new UserDTO
        {
            Id = appUser.User.Id,
            Email = appUser.User.Email,
            FullName = appUser.User.FullName,
            Role = appUser.User.Role,
            IdentityRoles = roles?.ToList() ?? [],
            RoleLabel = appUser.User.Role switch
            {
                Roles.Requester => "Requisitante",
                Roles.Manager => "Gerente",
                Roles.Director => "Diretor",
                Roles.Assistant => "Assistente",
                Roles.Finance => "Financeiro",
                _ => "Desconhecido"
            },
            Department = appUser.User.Department,
            DepartmentLabel = appUser.User.Department switch
            {
                Departaments.Engineer => "Engenharia",
                Departaments.Sales => "Vendas",
                Departaments.Operations => "Operações",
                Departaments.Marketing => "Marketing",
                Departaments.IT => "Tecnologia da Informação",
                Departaments.HR => "Recursos Humanos",
                Departaments.Finance => "Financeiro",
                _ => "Desconhecido"
            },
            PictureUrl = appUser.User.PictureUrl,
            Enabled = appUser.User.Enabled,
            RequestApproved = appUser.Approved,
        };
    }

    public static IApplicationUser Update(ref IApplicationUser user, UserUpdateDTO request)
    {
        user.UserName = request.Email;
        user.FullName = request.FullName;
        user.Email = request.Email;
        user.Role = request.Role;
        user.Department = request.Department;
        user.PictureUrl = request.PictureUrl;
        return user;
    }

}
