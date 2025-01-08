using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;

namespace WebAPI.Endpoints.Test;

public class TestModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("test".ToEndpointPrefix());

        group.MapGet("/", () =>
        {
            return Results.Ok("GET request received");
        }).AllowAnonymous();
    }
}
