using AprovaFacil.Application.Extensions;
using AprovaFacil.Application.Services;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using AprovaFacil.Infra.Data.Identity;
using AprovaFacil.Infra.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AprovaFacil.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext();

        services.AddServices();
        services.AddRepositories();

        return services;
    }

    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<Func<User, IApplicationUser>>(provider => user =>
        {
            return new ApplicationUser
            {
                Id = user.Id,
                UserName = user.Email,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Department = user.Department,
                PictureUrl = user.PictureUrl,
                Enabled = user.Enabled
            };
        });
        services.AddScoped<CompanyInterfaces.ICompanyService, CompanyService>();
        services.AddScoped<UserInterfaces.IUserService, UserService>();

        return services;
    }

    internal static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<CompanyInterfaces.ICompanyRepository, CompanyRepository>();
        services.AddScoped<UserInterfaces.IUserRepository, UserRepository>();
        return services;
    }

    internal static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        // Configurar DbContext
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

        // Configurar Identity com AddIdentityCore
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        }).AddRoles<IdentityRole<Int32>>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        // Adicionar serviços adicionais manualmente (necessário com AddIdentityCore)
        services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser, IdentityRole<Int32>, ApplicationDbContext, Int32>>();
        services.AddScoped<IRoleStore<IdentityRole<Int32>>, RoleStore<IdentityRole<Int32>, ApplicationDbContext, Int32>>();
        services.AddScoped<UserManager<ApplicationUser>>();
        services.AddScoped<SignInManager<ApplicationUser>>(); // Para login/logout
        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, IdentityRole<Int32>>>();

        // Registrar serviços da Application
        services.AddScoped<UserExtensions>();

        // Registrar IApplicationUser como ApplicationUser
        services.AddScoped<IApplicationUser>(provider =>
            provider.GetService<ApplicationUser>() ?? new ApplicationUser());

        return services;
    }

    public static void SeedData(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        RoleManager<IdentityRole<Int32>> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Int32>>>();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        List<Company> companies = [
        new Company
        {
            Id = 1,
            CNPJ = "12.345.678/0001-90",
            TradeName = "Tech Solutions",
            LegalName = "Tech Solutions Ltda",
            Address = new Address
            {
                PostalCode = "12345-678",
                State = "SP",
                City = "São Paulo",
                Neighborhood = "Centro",
                Street = "Avenida Paulista",
                Number = "1000",
                Complement = "Sala 101",
            },
            Phone = "(11) 98765-4321",
            Email = "contact@techsolutions.com"
        },
        new Company
            {
                Id = 2,
                CNPJ = "98.765.432/0001-23",
                TradeName = "Green Energy",
                LegalName = "Green Energy Soluções Sustentáveis Ltda",
                Address = new Address
                {
                    PostalCode = "87654-321",
                    State = "RJ",
                    City = "Rio de Janeiro",
                    Neighborhood = "Copacabana",
                    Street = "Rua Barata Ribeiro",
                    Number = "500",
                    Complement = "Apto 302",
                },
                Phone = "(21) 91234-5678",
                Email = "info@greenenergy.com.br"
            }
        ];

        foreach (Company company in companies)
        {
            Boolean exists = context.Companies.Where(c => c.Id == company.Id).Any();

            if (!exists)
            {
                context.Companies.Add(company);
            }
        }

        String[] roles = new[] { Roles.Requester, Roles.Manager, Roles.Director, Roles.Finance, Roles.Secretary };

        foreach (String? role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                roleManager.CreateAsync(new IdentityRole<Int32>(role));
            }
        }

        ApplicationUser[] users = new[]
            {
                new ApplicationUser
                {
                    UserName = "requester@example.com",
                    Email = "requester@example.com",
                    FullName = "Ana Requester",
                    Role = Roles.Requester,
                    Department = "TI",
                    PictureUrl = "/avatars/female/86.png",
                    Enabled = true
                },
                new ApplicationUser
                {
                    UserName = "manager@example.com",
                    Email = "manager@example.com",
                    FullName = "Bruno Manager",
                    Role = Roles.Manager,
                    Department = "Engenharia",
                    PictureUrl = "/avatars/male/47.png",
                    Enabled = true
                },
                new ApplicationUser
                {
                    UserName = "director@example.com",
                    Email = "director@example.com",
                    FullName = "Clara Director",
                    Role = Roles.Director,
                    Department = "Administração",
                    PictureUrl = "/avatars/female/82.png",
                    Enabled = true
                },
                new ApplicationUser
                {
                    UserName = "finance@example.com",
                    Email = "finance@example.com",
                    FullName = "Diego Finance",
                    Role = Roles.Finance,
                    Department = "Financeiro",
                    PictureUrl = "/avatars/male/19.png",
                    Enabled = true
                },
                new ApplicationUser
                {
                    UserName = "secretary@example.com",
                    Email = "secretary@example.com",
                    FullName = "Elisa Secretary",
                    Role = Roles.Secretary,
                    Department = "RH",
                    PictureUrl = "/avatars/female/100.png",
                    Enabled = true
                }
            };

        foreach (ApplicationUser user in users)
        {
            if (userManager.FindByEmailAsync(user.Email).Result == null)
            {
                IdentityResult result = userManager.CreateAsync(user, "Senha123").Result; // Senha padrão
                if (result.Succeeded)
                {
                    // Associar o usuário à role correspondente
                    String roleToAssign = roles[Array.IndexOf(users, user)];
                    userManager.AddToRoleAsync(user, roleToAssign).Wait();
                }
                else
                {
                    // Logar erros, se necessário
                    Console.WriteLine($"Erro ao criar {user.Email}: {String.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        context.SaveChanges();
    }
}
