using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Extensions;

public static class UserExtensions
{
    public static User ToDomainUser(this IApplicationUser appUser)
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

    public static UserDTO ToDTO(this IApplicationUser appUser)
    {
        return new UserDTO
        {
            Id = appUser.Id,
            Email = appUser.Email,
            FullName = appUser.FullName,
            Role = appUser.Role,
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

    public static IApplicationUser Update(ref IApplicationUser user, UserUpdateDTO request)
    {
        user.FullName = request.FullName;
        user.Email = request.Email;
        user.Role = request.Role;
        user.Department = request.Department;
        user.PictureUrl = request.PictureUrl;
        return user;
    }

    public static UserDTO ToDTO(this User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            RoleLabel = user.Role switch
            {
                Roles.Requester => "Requisitante",
                Roles.Manager => "Gerente",
                Roles.Director => "Diretor",
                Roles.Assistant => "Assistente",
                Roles.Finance => "Financeiro",
                _ => "Desconhecido"
            },
            Department = user.Department,
            DepartmentLabel = user.Department switch
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
            PictureUrl = user.PictureUrl,
            Enabled = user.Enabled,
        };
    }
}
