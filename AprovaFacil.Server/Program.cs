using AprovaFacil.Application.SignalR;
using AprovaFacil.Infra.IoC;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
                listenOptions.Protocols = HttpProtocols.Http1;
            });
        });

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
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
                .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "http://192.168.7.128:5173", "http://192.168.7.128:7296")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition");
            });
        });

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddSignalR();

        WebApplication app = builder.Build();

        app.UseDefaultFiles();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        DependencyInjection.SeedData(app.Services);

        app.UseCors("AllowLocalhost");

        //app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<NotificationHub>("/notification");

        app.Run();
    }
}
