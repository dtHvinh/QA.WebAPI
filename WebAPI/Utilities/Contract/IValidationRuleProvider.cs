using FluentValidation;

namespace WebAPI.Utilities.Contract;

public interface IValidationRuleProvider
{
    IRuleBuilderOptions<T, string> GetPasswordRule<T>(IRuleBuilderInitial<T, string> selector);
}