using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record AdminAnalyticQuery(string What, string Period) : IQuery<GenericResult<GrownAnalyticResponse>>;
