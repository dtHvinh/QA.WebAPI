namespace WebAPI.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RepositoryImplOfAttribute : Attribute
{
    public required Type Type { get; set; }
}
