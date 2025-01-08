namespace WebAPI.Utilities.Contract;

/// <summary>
/// The module contain method to register enpoint mapping which will automatically registerd at
/// application startup.
/// </summary>
public interface IModule
{
    void RegisterEndpoints(IEndpointRouteBuilder endpoints);
}
