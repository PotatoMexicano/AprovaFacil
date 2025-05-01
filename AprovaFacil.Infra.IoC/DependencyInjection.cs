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
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using static AprovaFacil.Domain.Interfaces.NotificationInterfaces;

namespace AprovaFacil.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.InitializeFolders(configuration);

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.Configure<SupabaseSettings>(configuration.GetSection("Supabase"));

        services.AddHttpClient("Supabase", (provider, client) =>
        {
            SupabaseSettings settings = provider.GetRequiredService<IOptions<SupabaseSettings>>().Value;
            client.BaseAddress = new Uri(settings.Endpoint);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.SecretKey);
        });

        services.AddDbContext(configuration);

        services.AddServices();
        services.AddRepositories();

        return services;
    }

    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<CompanyInterfaces.ICompanyService, CompanyService>();
        services.AddScoped<UserInterfaces.IUserService, UserService>();
        services.AddScoped<RequestInterfaces.IRequestService, RequestService>();

        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<ITenantProvider, TenantProvider>();

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
            options.UseNpgsql(configuration.GetConnectionString("Default"), opt =>
            {
                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                opt.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(50),
                    errorCodesToAdd: null);
            }), ServiceLifetime.Scoped
        );

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

    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        RoleManager<IdentityRole<Int32>> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Int32>>>();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        //await context.Database.MigrateAsync();

        Tenant tenant = new Tenant
        {
            Id = 1,
            Name = "Embraplan",
            Email = "email@embraplan.com",
            PhoneNumber = "(19) 99897-0000",
            CNPJ = "0394853653",
            Address = new Address
            {
                City = "Piracicaba",
                Number = "1200",
                State = "São Paulo",
                PostalCode = "13400-765",
                Street = "R. Tiradentes",
                Neighborhood = "Centro",
                Complement = String.Empty
            },
            ContactPerson = "Luciano Cordeiro"
        };

        if (!await context.Tenants.AnyAsync(t => t.Id == tenant.Id))
            await context.Tenants.AddAsync(tenant);

        List<Company> companies = new List<Company>
    {
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
            Email = "contact@techsolutions.com",
            TenantId = 1,
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
            Email = "info@greenenergy.com.br",
            TenantId = 1,
        }
    };

        foreach (Company company in companies)
        {
            if (!await context.Companies.AnyAsync(c => c.Id == company.Id))
                await context.Companies.AddAsync(company);
        }

        await context.SaveChangesAsync();

        String[] roles = new[] { Roles.Requester, Roles.Manager, Roles.Director, Roles.Finance, Roles.Assistant };

        foreach (String? role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Int32>(role));
            }
        }

        ApplicationUser[] users = new[]
        {
        new ApplicationUser("ana@example.com", "Ana Luiza", Roles.Requester, Departaments.IT, 1, Avatars.Female86),
        new ApplicationUser("bruno@example.com", "Bruno Lima", Roles.Manager, Departaments.Engineer, 1, Avatars.Male47),
        new ApplicationUser("marcos@example.com", "Marcos Souza", Roles.Requester, Departaments.IT, 1, Avatars.Male24, false),
        new ApplicationUser("clara@example.com", "Clara dias", Roles.Director, Departaments.Sales, 1, Avatars.Female82),
        new ApplicationUser("diego@example.com", "Diego Felippe", Roles.Finance, Departaments.Finance, 1, Avatars.Male19),
        new ApplicationUser("elisa@example.com", "Elisa Lucca", Roles.Assistant, Departaments.HR, 1, Avatars.Female100),
    };

        foreach (ApplicationUser? user in users)
        {
            ApplicationUser? existingUser = await userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                IdentityResult result = await userManager.CreateAsync(user, "Senha123"); // Senha padrão
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, user.Role);
                }
                else
                {
                    Console.WriteLine($"Erro ao criar {user.Email}: {String.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        await context.SaveChangesAsync();
    }

    private static void InitializeFolders(this IServiceCollection services, IConfiguration configuration)
    {
        String? invoicePath = Path.Combine(configuration.GetValue<String>("Directory:Invoices") ?? "invoices");
        String? budgetPath = Path.Combine(configuration.GetValue<String>("Directory:Budgets") ?? "budgets");

        //if (!Directory.Exists(invoicePath))
        //{
        //    Directory.CreateDirectory(invoicePath);
        //}

        //if (!Directory.Exists(budgetPath))
        //{
        //    Directory.CreateDirectory(budgetPath);
        //}

        services.AddSingleton(x => new ServerDirectory
        {
            InvoicePath = invoicePath,
            BudgetPath = budgetPath,
        });
    }
}
