using System.Reflection;
using WebAPI.Utilities.Attributes;

namespace WebAPI.Utilities.Auto;

public static class RepositoryRegistrationExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        var types = Assembly.GetAssembly(typeof(RepositoryImplAttribute))?
              .GetTypes()
              .Where(x => x.GetCustomAttributes<RepositoryImplAttribute>().Any())
              .ToList()!;

        foreach (var type in types)
        {
            var repositoryType = type.GetCustomAttribute<RepositoryImplAttribute>()?.Type;
            if (repositoryType is not null)
            {
                services.AddScoped(repositoryType, type);
            }
        }

        return services;
    }
}
