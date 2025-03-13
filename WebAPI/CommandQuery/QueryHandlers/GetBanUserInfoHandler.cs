using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetBanUserInfoHandler(ICacheService cacheService)
    : IQueryHandler<GetBanUserInfoQuery, GenericResult<BanInfoResponse>>
{
    private readonly ICacheService _cacheService = cacheService;

    public async Task<GenericResult<BanInfoResponse>> Handle(GetBanUserInfoQuery request, CancellationToken cancellationToken)
    {
        var banEndDate = await _cacheService.IsBannedWithReason(request.UserId, cancellationToken);

        if (banEndDate == null)
        {
            return GenericResult<BanInfoResponse>.Failure("User is not banned");
        }

        return GenericResult<BanInfoResponse>.Success(new BanInfoResponse(banEndDate.Value.Item1, banEndDate.Value.Item2));
    }
}
