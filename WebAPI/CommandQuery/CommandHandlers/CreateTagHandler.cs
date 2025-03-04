using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateTagHandler(
    ITagRepository tagRepository,
    Serilog.ILogger logger)
    : ICommandHandler<CreateTagCommand, GenericResult<GenericResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<GenericResponse>> Handle(
        CreateTagCommand request, CancellationToken cancellationToken)
    {
        var newTag = request.Tag.ToTag();

        _tagRepository.CreateTag(newTag);

        var createTag = await _tagRepository.SaveChangesAsync(cancellationToken);
        if (!createTag.IsSuccess)
        {
            return GenericResult<GenericResponse>.Failure(createTag.Message);
        }

        _logger.Information("Tag {TagName} created with Id {TagId}", newTag.Name, newTag.Id);

        return GenericResult<GenericResponse>.Success("Ok");
    }
}