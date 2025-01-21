
namespace WebAPI.Utilities.Contract;

public interface IResourceRight<TResource>
{
    string ResourceRight { get; set; }

    TResource SetResourceRight(Guid requuesterId);
}

public enum ResourceRights
{
    Owner,
    Viewer
}


