using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetTagDescriptionQuery(int TagId) :
    IQuery<GenericResult<TextResponse>>;
