using AprovaFacil.Application.Services;
using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using AprovaFacil.Infra.Data.Identity;
using AprovaFacil.Infra.Data.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static AprovaFacil.Domain.Interfaces.NotificationInterfaces;

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
        services.AddScoped<CompanyInterfaces.ICompanyService, CompanyService>();
        services.AddScoped<UserInterfaces.IUserService, UserService>();
        services.AddScoped<RequestInterfaces.IRequestService, RequestService>();
        services.AddScoped<INotificationService, NotificationService>();

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
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite("Data Source=E:\\Desenvolvimento\\CSharp\\AprovaFacil\\AprovaFacil.Server\\mydb.db",
                opt => opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });

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

        services.ConfigureApplicationCookie(o =>
        {
            o.Events = new CookieAuthenticationEvents()
            {
                OnRedirectToLogin = (ctx) =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                    {
                        ctx.Response.StatusCode = 401;
                    }

                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = (ctx) =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                    {
                        ctx.Response.StatusCode = 403;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        }).AddCookie(options =>
        {
            options.Cookie.Domain = "192.168.7.128";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.ExpireTimeSpan = TimeSpan.FromHours(1);
            options.LoginPath = "/api/auth/login";
            options.SlidingExpiration = true;
        });

        services.AddAuthorization();

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
            new ApplicationUser("ana@example.com", "Ana Luiza", Roles.Requester, Departaments.IT, Avatars.Female86),
            new ApplicationUser("bruno@example.com", "Bruno Lima", Roles.Manager, Departaments.Engineer, Avatars.Male47),
            new ApplicationUser("marcos@example.com", "Marcos Souza", Roles.Requester, Departaments.IT, Avatars.Male24, false),
            new ApplicationUser("clara@example.com", "Clara dias", Roles.Director, Departaments.Sales, Avatars.Female82),
            new ApplicationUser("diego@example.com", "Diego Felippe", Roles.Finance, Departaments.Finance, Avatars.Male19),
            new ApplicationUser("elisa@example.com", "Elisa Lucca", Roles.Assistant, Departaments.HR, Avatars.Female100),
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

        services.AddSingleton(x => new ServerDirectory
        {
            InvoicePath = invoicePath,
            BudgetPath = budgetPath,
        });
    }
}
