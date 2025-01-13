using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ImplementAttribute(Type type, ServiceLifetime lifetime) : Attribute
{
    public Type Type { get; set; } = type;
    public ServiceLifetime Lifetime { get; set; } = lifetime;
}
