using AprovaFacil.Application.SignalR;
using AprovaFacil.Infra.IoC;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Exceptions;
using System.Net;

namespace AprovaFacil.Server;

public class Program
{
    public static async Task Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 7296, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });
        });

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .CreateLogger();

        builder.Host.UseSerilog(Log.Logger);

        builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", policy =>
            {
                policy
                .WithOrigins("http://192.168.7.128:7296", "http://192.168.7.128:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition");
            });
        });

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddSignalR();

        WebApplication app = builder.Build();

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapFallbackToFile("index.html");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (IServiceScope scope = app.Services.CreateScope())
        {
            IServiceProvider services = scope.ServiceProvider;
            await DependencyInjection.SeedDataAsync(services);
        }

        app.UseCors("AllowLocalhost");

        //app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<NotificationHub>("/notification");

        app.Run();
    }
}
