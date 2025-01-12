namespace WebAPI.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RepositoryImplAttribute(Type type) : Attribute
{
    public Type Type { get; set; } = type;
}
