using AprovaFacil.Application.Extensions;
using AprovaFacil.Application.Services;
using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using AprovaFacil.Infra.Data.Identity;
using AprovaFacil.Infra.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AprovaFacil.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.InitializeFolders(configuration);

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddDbContext(configuration);

        services.AddServices();
        services.AddRepositories();

        return services;
    }

    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<JwtService>();
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
        services.AddScoped<RequestInterfaces.IRequestService, RequestService>();

        return services;
    }

    internal static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<CompanyInterfaces.ICompanyRepository, CompanyRepository>();
        services.AddScoped<UserInterfaces.IUserRepository, UserRepository>();
        services.AddScoped<RequestInterfaces.IRequestRepository, RequestRepository>();
        return services;
    }

    internal static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        // Configurar DbContext
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

        // Configurar Identity com AddIdentityCore
        services.AddIdentity<ApplicationUser, IdentityRole<Int32>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddSignInManager<SignInManager<ApplicationUser>>();

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<String>("Jwt:Key")));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Usar JWT em vez de cookies
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
        });

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

        String[] roles = new[] { Roles.Requester, Roles.Manager, Roles.Director, Roles.Finance, Roles.Assistant };

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
                    UserName = "disabled@example.com",
                    Email = "disabled@example.com",
                    FullName = "Marcos Disabled",
                    Role = Roles.Requester,
                    Department = "TI",
                    PictureUrl = "/avatars/female/86.png",
                    Enabled = false
                },
                new ApplicationUser
                {
                    UserName = "manager@example.com",
                    Email = "manager@example.com",
                    FullName = "Bruno User",
                    Role = Roles.Manager,
                    Department = "Engenharia",
                    PictureUrl = "/avatars/male/47.png",
                    Enabled = true
                },
                new ApplicationUser
                {
                    UserName = "director@example.com",
                    Email = "director@example.com",
                    FullName = "Clara User",
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
                    FullName = "Elisa Assistant",
                    Role = Roles.Assistant,
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
                    String roleToAssign = user.Role;
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

    private static void InitializeFolders(this IServiceCollection services, IConfiguration configuration)
    {
        String? invoicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration.GetValue<String>("Directory:Invoices"));
        String? budgetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration.GetValue<String>("Directory:Budgets"));

        if (!Directory.Exists(invoicePath))
        {
            Directory.CreateDirectory(invoicePath);
        }

        if (!Directory.Exists(budgetPath))
        {
            Directory.CreateDirectory(budgetPath);
        }

        services.AddSingleton<ServerDirectory>(x => new ServerDirectory
        {
            InvoicePath = invoicePath,
            BudgetPath = budgetPath,
        });
    }
}
