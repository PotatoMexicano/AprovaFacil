using AprovaFacil.Infra.IoC;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Exceptions;
using System.Net;

namespace AprovaFacil.Server;

public class Program
{
    public static void Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 7296, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
                listenOptions.UseHttps(@"E:\Desenvolvimento\CSharp\AprovaFacil\certificado\localhost.pfx", "12345678");
            });

            options.Listen(IPAddress.Any, 5118, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });

        });

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .CreateLogger();

        builder.Host.UseSerilog(Log.Logger);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", policy =>
            {
                policy.WithOrigins("https://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme);

        builder.Services.AddAuthorization();

        WebApplication app = builder.Build();

        app.UseDefaultFiles();
        app.MapStaticAssets();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(opt => opt.AddServer(new ScalarServer("https://localhost:7296")));
        }

        DependencyInjection.SeedData(app.Services);

        app.UseCors("AllowLocalhost");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
