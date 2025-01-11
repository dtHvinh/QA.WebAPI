using FluentValidation;
using WebAPI.Utilities.Contract;

namespace WebAPI.Utilities.Provider;

public class ValidationRuleProvider : IValidationRuleProvider
{
    private readonly int _passwordMinLength = 6;

    public IRuleBuilderOptions<T, string> GetPasswordRule<T>(IRuleBuilderInitial<T, string> builderInitial)
    {
        return builderInitial.MinimumLength(_passwordMinLength);
    }
}
