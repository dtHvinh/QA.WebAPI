using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateProfileImageHandler(
    StorageService storageService,
    AuthenticationContext authenticationContext,
    IUserRepository userRepository)
    : ICommandHandler<UpdateProfileImageCommand, GenericResult<TextResponse>>
{
    private readonly StorageService _storageService = storageService;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<GenericResult<TextResponse>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindUserByIdAsync(_authenticationContext.UserId, cancellationToken);

        if (user is null)
        {
            return GenericResult<TextResponse>.Failure("User not found");
        }

        if (_authenticationContext.UserId != user.Id)
        {
            return GenericResult<TextResponse>.Failure("You a not allowed to do this");
        }

        var path = await _storageService.UploadUserPfp(user.Id, request.File, cancellationToken);

        user.ProfilePicture = path;

        _userRepository.Update(user);

        var res = await _userRepository.SaveChangesAsync(cancellationToken);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success(path)
            : GenericResult<TextResponse>.Failure("Failed to update profile image");
    }
}
