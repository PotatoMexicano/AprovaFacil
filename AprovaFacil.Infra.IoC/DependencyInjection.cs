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
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUnitOfWorkInterface, UnitOfWorkRepository>();
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
        ITenantRepository tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();

        //await context.Database.MigrateAsync();

        Tenant? tenant = await tenantRepository.GetByIdAsync(1, CancellationToken.None); // Check if tenant exists
        if (tenant == null)
        {
            tenant = new Tenant
            {
                Id = 1, // Explicitly set Id for seeding if it's not auto-generated or if you want a specific Id
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
                ContactPerson = "Luciano Cordeiro",
                Plan = PlanType.Free // Example: Set a plan
            };
            tenant.SetLimitsBasedOnPlan(); // Set limits based on the plan
            await context.Tenants.AddAsync(tenant);
            await context.SaveChangesAsync(); // Save tenant to get Id if auto-generated, and to persist
        }
        else
        {
            // If tenant exists, ensure limits are set (e.g., if this code runs after model changes)
            tenant.SetLimitsBasedOnPlan();
            await tenantRepository.UpdateAsync(tenant, CancellationToken.None);
        }

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
            TenantId = tenant.Id, // Use the Id of the seeded/fetched tenant
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
            TenantId = tenant.Id, // Use the Id of the seeded/fetched tenant
        }
    };

        foreach (Company company in companies)
        {
            if (!await context.Companies.AnyAsync(c => c.Id == company.Id && c.TenantId == tenant.Id))
            {
                company.TenantId = tenant.Id; // Ensure TenantId is set
                await context.Companies.AddAsync(company);
            }
        }

        await context.SaveChangesAsync();

        String[] roles = new[] { Roles.Requester, Roles.Manager, Roles.Director, Roles.Finance, Roles.Assistant };

        foreach (String? roleName in roles) // Changed variable name for clarity
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Int32>(roleName));
            }
        }

        ApplicationUser[] users = new[]
        {
        new ApplicationUser("ana@example.com", "Ana Luiza", Roles.Requester, Departaments.IT, tenant.Id, Avatars.Female86),
        new ApplicationUser("bruno@example.com", "Bruno Lima", Roles.Manager, Departaments.Engineer, tenant.Id, Avatars.Male47),
        new ApplicationUser("marcos@example.com", "Marcos Souza", Roles.Requester, Departaments.IT, tenant.Id, Avatars.Male24, false),
        new ApplicationUser("clara@example.com", "Clara dias", Roles.Director, Departaments.Sales, tenant.Id, Avatars.Female82),
        new ApplicationUser("diego@example.com", "Diego Felippe", Roles.Finance, Departaments.Finance, tenant.Id, Avatars.Male19),
        new ApplicationUser("elisa@example.com", "Elisa Lucca", Roles.Assistant, Departaments.HR, tenant.Id, Avatars.Female100),
    };

        foreach (ApplicationUser? userSeedInfo in users) // Changed variable name for clarity
        {
            ApplicationUser? existingUser = await userManager.FindByEmailAsync(userSeedInfo.Email);
            if (existingUser == null)
            {
                userSeedInfo.TenantId = tenant.Id; // Ensure TenantId is set before creation
                IdentityResult result = await userManager.CreateAsync(userSeedInfo, "Senha123"); // Senha padrão
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(userSeedInfo, userSeedInfo.Role);
                }
                else
                {
                    Console.WriteLine($"Erro ao criar {userSeedInfo.Email}: {String.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        await context.SaveChangesAsync();
    }

    private static void InitializeFolders(this IServiceCollection services, IConfiguration configuration)
    {
        String? invoicePath = Path.Combine(configuration.GetValue<String>("Directory:Invoices") ?? "invoices");
        String? budgetPath = Path.Combine(configuration.GetValue<String>("Directory:Budgets") ?? "budgets");

        services.AddSingleton(x => new ServerDirectory
        {
            InvoicePath = invoicePath,
            BudgetPath = budgetPath,
        });
    }
}

