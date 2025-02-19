using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dto;
using WebAPI.Filters.Validation;
using WebAPI.Response.CommentResponses;
using WebAPI.Utilities.Contract;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Collection;

public class CollectionModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Collection);

        group.MapPost("/", HandleCreateCollection)
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCollectionDto>>();
    }

    private Task<Results<Ok<CommentResponse>, ProblemHttpResult>> HandleCreateCollection(
        [FromBody] CreateCollectionDto dto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
