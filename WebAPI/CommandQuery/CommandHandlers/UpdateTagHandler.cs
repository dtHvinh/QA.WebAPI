using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateTagHandler(ITagRepository tagRepository, AuthenticationContext authenticationContext)
    : ICommandHandler<UpdateTagCommand, GenericResult<GenericResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(
        UpdateTagCommand request, CancellationToken cancellationToken)
    {
        if (!_authenticationContext.IsModerator())
            return GenericResult<GenericResponse>.Failure(string.Format(Constants.EM.ROLE_NOT_MEET_REQ,
                nameof(Constants.Roles.Moderator)));

        var newTag = request.Tag.ToTag();

        _tagRepository.Update(newTag);

        var result = await _tagRepository.SaveChangesAsync(cancellationToken);

        return !result.IsSuccess
            ? GenericResult<GenericResponse>.Failure(result.Message)
            : GenericResult<GenericResponse>.Success("OK");
    }
}