using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class AdminGetUserHandler(IAdminRepository adminRepository, IUserRepository userRepository, ICacheService cacheService)
    : IQueryHandler<AdminGetUserQuery, GenericResult<PagedResponse<GetUserResponse>>>
{
    private readonly IAdminRepository _adminRepository = adminRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<GenericResult<PagedResponse<GetUserResponse>>> Handle(AdminGetUserQuery request, CancellationToken cancellationToken)
    {
        var users = await _adminRepository.GetUsers(request.PageArgs.CalculateSkip(), request.PageArgs.PageSize + 1, cancellationToken);

        var hasNext = users.Count == request.PageArgs.PageSize + 1;

        var totalCount = await _userRepository.CountAsync();

        var res = users
            .Take(request.PageArgs.PageSize)
            .Select(e => e.ToGetUserResponse().SetIsBanned(_cacheService.IsBanned(e.Id)));

        return GenericResult<PagedResponse<GetUserResponse>>.Success(
            new PagedResponse<GetUserResponse>(
                res,
                hasNext,
                request.PageArgs.PageIndex,
                request.PageArgs.PageSize
            )
            {
                TotalCount = totalCount,
                TotalPage = MathHelper.GetTotalPage(totalCount, request.PageArgs.PageSize)
            }
        );
    }
}
