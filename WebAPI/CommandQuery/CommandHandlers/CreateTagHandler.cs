using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateTagHandler(ITagRepository tagRepository, AuthenticationContext authenticationContext)
    : ICommandHandler<CreateTagCommand, GenericResult<GenericResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(
        CreateTagCommand request, CancellationToken cancellationToken)
    {
        if (!_authenticationContext.IsModerator())
            return GenericResult<GenericResponse>.Failure(string.Format(Constants.EM.ROLE_NOT_MEET_REQ,
                nameof(Constants.Roles.Moderator)));

        var newTag = request.Tag.ToTag();

        _tagRepository.CreateTag(newTag);

        var createTag = await _tagRepository.SaveChangesAsync(cancellationToken);
        if (!createTag.IsSuccess)
        {
            return GenericResult<GenericResponse>.Failure(createTag.Message);
        }

        return GenericResult<GenericResponse>.Success("Ok");
    }
}