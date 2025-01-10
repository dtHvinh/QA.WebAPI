using CQRS;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class LoginQueryHandler(IUserRepository userRepository) : IQueryHandler<LoginQuery, ResultBase<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public Task<ResultBase<AuthResponseDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
