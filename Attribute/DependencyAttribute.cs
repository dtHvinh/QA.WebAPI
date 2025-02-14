using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DependencyAttribute : Attribute
{
    public Type? Type { get; set; }
    public ServiceLifetime Lifetime { get; set; }

    public DependencyAttribute(Type? type, ServiceLifetime lifetime)
    {
        Type = type;
        Lifetime = lifetime;
    }

    public DependencyAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}
