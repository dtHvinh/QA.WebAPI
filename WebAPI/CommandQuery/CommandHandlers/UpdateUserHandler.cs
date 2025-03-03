using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateUserHandler(
    IUserRepository userRepository,
    AuthenticationContext authenticationContext)
    : ICommandHandler<UpdateUserCommand, GenericResult<GenericResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindUserWithLinks(_authenticationContext.UserId, cancellationToken);

        if (user is null)
        {
            return GenericResult<GenericResponse>.Failure("User not found");
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

        return updateRes.IsSuccess
            ? GenericResult<GenericResponse>.Success("User updated")
            : GenericResult<GenericResponse>.Failure(updateRes.Message);
    }
}
