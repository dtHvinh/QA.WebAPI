
namespace WebAPI.Utilities.Contract;

public interface IResourceRight<TResource>
{
    string ResourceRight { get; set; }

    TResource SetResourceRight(Guid? requesterId);
}

public enum ResourceRights
{
    Owner,
    Viewer
}


