using AprovaFacil.Infra.Data.Context;
using AprovaFacil.Infra.IoC;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Exceptions;

namespace AprovaFacil.Server;

public class Program
{
    public static void Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

        WebApplication app = builder.Build();

        app.UseDefaultFiles();
        app.MapStaticAssets();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        using (IServiceScope scope = app.Services.CreateScope())
        {
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            DependencyInjection.SeedData(dbContext);
        }

        app.UseCors("AllowLocalhost");

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
