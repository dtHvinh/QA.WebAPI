using System.Reflection;
using WebAPI.Utilities.Contract;

namespace WebAPI.Utilities.Auto;

public static class ModuleRegistration
{
    private static IEnumerable<Type> GetModules()
        => Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(x => x.GetInterfaces().Contains(typeof(IModule)));

    public static void RegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        foreach (var module in GetModules())
        {
            var instance = Activator.CreateInstance(module) as IModule
                ?? throw new InvalidOperationException($"Failed to create instance of {module.Name}");

            instance.RegisterEndpoints(endpoints);
        }
    }
}
