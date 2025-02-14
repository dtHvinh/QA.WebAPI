using System.Reflection;
using WebAPI.Attributes;

namespace WebAPI.Utilities.Reflection;

public static class ServiceRegistration
{
    public static IServiceCollection AutoRegisterAllServices(this IServiceCollection services)
    {
        var types = Assembly.GetExecutingAssembly()?
              .GetTypes()
              .Where(x => x.GetCustomAttributes<DependencyAttribute>().Any())
              .ToList()!;

        foreach (var type in types)
        {
            var repositoryType = type.GetCustomAttribute<DependencyAttribute>()!.Type;
            var lifetime = type.GetCustomAttribute<DependencyAttribute>()!.Lifetime;

            if (repositoryType is not null)
            {
                services.Add(new ServiceDescriptor(repositoryType, type, lifetime));
            }
            else
                services.Add(new ServiceDescriptor(type, lifetime));
        }

        return services;
    }
}
