using Microsoft.Extensions.DependencyInjection;

namespace EquipTrack.Query.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<Class1>()
            .AddClasses(classes => classes.Where(type => 
                type.Name.EndsWith("QueryService") ||
                type.Name.EndsWith("QueryHandler")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}