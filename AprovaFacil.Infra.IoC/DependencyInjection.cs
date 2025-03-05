using AprovaFacil.Application.Services;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using AprovaFacil.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AprovaFacil.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddServices();
        services.AddRepositories();

        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

        return services;
    }

    internal static IServiceCollection AddServices(this IServiceCollection services)
    {

        services.AddScoped<CompanyInterfaces.ICompanyService, CompanyService>();

        return services;
    }

    internal static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<CompanyInterfaces.ICompanyRepository, CompanyRepository>();
        return services;
    }

    public static void SeedData(ApplicationDbContext context)
    {
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

        context.SaveChanges();
    }
}
