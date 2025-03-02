using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CollectionQuestionOperationHandler(
    ICollectionRepository repository,
    IQuestionRepository questionRepository,
    AuthenticationContext authenticationContext)
    : ICommandHandler<CollectionQuestionOperationCommand, GenericResult<GenericResponse>>
{
    private readonly ICollectionRepository _repository = repository;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(
        CollectionQuestionOperationCommand request, CancellationToken cancellationToken)
    {
        var collection = await _repository.FindByIdAsync(request.CollectionId, cancellationToken);

        if (collection is null)
            return GenericResult<GenericResponse>.Failure("Collection not found");

        if (!_authenticationContext.IsResourceOwnedByUser(collection))
            return GenericResult<GenericResponse>.Failure(EM.ACTION_REQUIRE_RESOURCE_OWNER);

        var question = await _questionRepository.FindQuestionById(request.QuestionId, cancellationToken);

        if (question is null)
            return GenericResult<GenericResponse>.Failure("Question not found");

        switch (request)
        {
            case { Operation: Utilities.Operations.Add }:
                _repository.AddToCollection(question, collection);
                collection.QuestionCount++;
                break;
            case { Operation: Utilities.Operations.Delete }:
                _repository.RemoveFromCollection(question, collection);
                collection.QuestionCount--;
                break;
        }

        var res = await _questionRepository.SaveChangesAsync(cancellationToken);

        return res.IsSuccess
            ? GenericResult<GenericResponse>.Success("Done")
            : GenericResult<GenericResponse>.Failure(res.Message);
    }
}