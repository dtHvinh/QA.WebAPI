namespace WebAPI.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RepositoryImplAttribute(Type type) : Attribute
{
    public Type Type { get; set; } = type;
}
