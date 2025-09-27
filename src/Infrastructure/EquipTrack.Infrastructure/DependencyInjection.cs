using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using EquipTrack.Application.Interfaces;
using EquipTrack.Domain.Common;
using EquipTrack.Domain.Repositories;
using EquipTrack.Infrastructure.Data;
using EquipTrack.Infrastructure.Repositories;
using EquipTrack.Infrastructure.Services;
using System.Reflection;

namespace EquipTrack.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer.
/// Implements clean architecture principles with automatic service registration using Scrutor.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext with proper configuration
        services.AddDbContext<EquipTrackDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.EnableSensitiveDataLogging(false);
            options.EnableServiceProviderCaching();
            options.EnableDetailedErrors(false);
        });

        // Add Unit of Work - Clean architecture pattern for transaction management
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Auto-register all repositories using Scrutor
        // This follows the convention-based registration pattern for enterprise applications
        services.Scan(scan => scan
            .FromAssemblyOf<AssetReadOnlyRepository>()
            .AddClasses(classes => classes
                .Where(type => type.Name.EndsWith("Repository") && 
                              !type.IsAbstract && 
                              !type.IsInterface))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register essential infrastructure services
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtService, JwtService>();

        // Auto-register application services using Scrutor
        // This ensures all services following naming conventions are automatically registered
        services.Scan(scan => scan
            .FromAssemblyOf<AssetService>()
            .AddClasses(classes => classes
                .Where(type => type.Name.EndsWith("Service") && 
                              !type.IsAbstract && 
                              !type.IsInterface &&
                              type.Namespace?.Contains("Infrastructure.Services") == true))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    /// <summary>
    /// Adds health checks for infrastructure components.
    /// Essential for enterprise-grade applications with monitoring requirements.
    /// </summary>
    public static IServiceCollection AddInfrastructureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found"));

        return services;
    }

    /// <summary>
    /// Configures Entity Framework for optimal performance in enterprise environments.
    /// </summary>
    public static IServiceCollection AddOptimizedEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<EquipTrackDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(30);
            });
            
            // Production optimizations
            options.EnableSensitiveDataLogging(false);
            options.EnableServiceProviderCaching();
            options.EnableDetailedErrors(false);
            options.ConfigureWarnings(warnings => warnings.Ignore());
        });

        return services;
    }
}