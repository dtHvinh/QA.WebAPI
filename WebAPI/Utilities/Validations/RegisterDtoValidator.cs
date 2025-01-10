using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using WebAPI.Dto;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Utilities.Validations;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(IDistributedCache cache)
    {
        RuleFor(e => e.Email).EmailAddress().MustAsync(async (email, canceleation) =>
        {
            return await cache.GetAsync(RedisKeyGen.ForEmailDuplicate(email), canceleation) is null;
        }).WithMessage("Email has been used!!!");

        RuleFor(e => e.Password).MinimumLength(6);
    }
}
