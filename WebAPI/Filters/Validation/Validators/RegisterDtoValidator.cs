using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using WebAPI.Dto;
using WebAPI.Utilities.Contract;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Filters.Validation.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(IDistributedCache cache, IValidationRuleProvider ruleProvider)
    {
        RuleFor(e => e.Email).EmailAddress().MustAsync(async (email, canceleation) =>
        {
            return await cache.GetAsync(RedisKeyGen.UserEmail(email), canceleation) is null;
        }).WithMessage("Email has been used!!!");

        ruleProvider.GetPasswordRule(RuleFor(e => e.Password));
    }
}
