using System.Reflection;
using WebAPI.Utilities.Attributes;

namespace WebAPI.Utilities.Auto;

public static class RegisterRepository
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        var types = Assembly.GetAssembly(typeof(RepositoryImplOfAttribute))?
              .GetTypes()
              .Where(x => x.GetCustomAttributes<RepositoryImplOfAttribute>().Any())
              .ToList()!;

        foreach (var type in types)
        {
            var repositoryType = type.GetCustomAttribute<RepositoryImplOfAttribute>()?.Type;
            if (repositoryType is not null)
            {
                services.AddScoped(repositoryType, type);
            }
        }

        return services;
    }
}
