using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateUserHandler(
    IUserRepository userRepository,
    AuthenticationContext authenticationContext,
    StorageService storage,
    Serilog.ILogger logger)
    : ICommandHandler<UpdateUserCommand, GenericResult<TextResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly StorageService _storage = storage;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindUserWithLinks(_authenticationContext.UserId, cancellationToken);

        if (user is null)
        {
            return GenericResult<TextResponse>.Failure("User not found");
        }

        if (request.UpdateUserDto.Username is not null)
            user.UserName = request.UpdateUserDto.Username;

        user.ExternalLinks = request.UpdateUserDto.Links.Select(e => new ExternalLinks()
        {
            AuthorId = _authenticationContext.UserId,
            Provider = e.Provider,
            Url = e.Url
        }).ToList();

        _userRepository.Update(user);

        var updateRes = await _userRepository.SaveChangesAsync(cancellationToken);

        if (updateRes.IsSuccess)
            _logger.Information("User with id: {UserId} updated", user.Id);

        return updateRes.IsSuccess
            ? GenericResult<TextResponse>.Success("User updated")
            : GenericResult<TextResponse>.Failure(updateRes.Message);
    }
}
