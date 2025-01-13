using System.Reflection;
using WebAPI.Attributes;

namespace WebAPI.Utilities.Auto;

public static class ServiceRegistration
{
    public static IServiceCollection AutoRegisterAllServices(this IServiceCollection services)
    {
        var types = Assembly.GetExecutingAssembly()?
              .GetTypes()
              .Where(x => x.GetCustomAttributes<ImplementAttribute>().Any())
              .ToList()!;

        foreach (var type in types)
        {
            var repositoryType = type.GetCustomAttribute<ImplementAttribute>()!.Type;
            var lifetime = type.GetCustomAttribute<ImplementAttribute>()!.Lifetime;

            if (repositoryType is not null)
            {
                services.Add(new ServiceDescriptor(repositoryType, type, lifetime));
            }
        }

        return services;
    }
}
