using System.Security.Claims;
using WebAPI.Utilities.Contract;

namespace WebAPI.Utilities.Context;

public class AuthenticationContext(IHttpContextAccessor hca)
{
    private readonly HttpContext? _httpContext = hca.HttpContext;

    public bool IsResourceOwnedByUser(IOwnedByUser<Guid> resource) => UserId == resource.AuthorId;

    public Guid UserId => Guid.Parse(_httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
    public string Role => _httpContext?.User?.FindFirst(ClaimTypes.Role)!.Value!;
}
