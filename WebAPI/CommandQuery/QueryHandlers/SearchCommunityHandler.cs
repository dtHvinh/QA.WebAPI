using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class SearchCommunityHandler(
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : IQueryHandler<SearchCommunityQuery, GenericResult<List<GetCommunityResponse>>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<List<GetCommunityResponse>>> Handle(SearchCommunityQuery request, CancellationToken cancellationToken)
    {
        var res = await _communityRepository.Search(
            _authenticationContext.UserId,
            request.SearchTerm,
            request.PageArgs.CalculateSkip(),
            request.PageArgs.PageSize, cancellationToken);

        _logger.Information("Community search with keyword {Keyword} has at least {Count} results",
            request.SearchTerm, res.Count);

        return GenericResult<List<GetCommunityResponse>>.Success(
            res.Select(e => e.ToResponse().WithIsJoined(e.IsJoined)).ToList());
    }
}
