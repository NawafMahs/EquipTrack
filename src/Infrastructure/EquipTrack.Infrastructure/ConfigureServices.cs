using EquipTrack.Application.Common.Interfaces;
using EquipTrack.Core.SharedKernel;
using EquipTrack.Domain.Entities;
using EquipTrack.Infrastructure.Data;
using EquipTrack.Infrastructure.Repositories;
using EquipTrack.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace EquipTrack.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer.
/// Implements clean architecture principles with automatic service registration using Scrutor.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Adds the infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddSignInManager()
               .AddDefaultTokenProviders();

        return services;
    }


    /// <summary>
    /// Adds the write-only repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddWriteOnlyRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IWriteOnlyRepository<,>), (typeof(BaseWriteOnlyRepository<,>)));
        services.Scan(selector => selector
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.Where(c => c.Name.EndsWith("WriteOnlyRepository")), publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .AsSelf()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddChacheService(this IServiceCollection services)
    {
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 10240; // Make sure the size limit is large enough
        });
        services
            .AddMemoryCache();

       

        return services;
    }

}