using System.Reflection;
using WebAPI.Utilities.Contract;

namespace WebAPI.Auto;

public static class RegisterModule
{
    private static IEnumerable<Type> GetModules()
        => Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(x => x.GetInterfaces()
        .Contains(typeof(IModule)));

    public static void RegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        foreach (var module in GetModules())
        {
            var instance = Activator.CreateInstance(module) as IModule
                ?? throw new Exception($"Failed to create instance of {module.Name}");

            instance.RegisterEndpoints(endpoints);
        }
    }
}
