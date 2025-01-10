using FluentValidation;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Filters.Validation;

public class FluentValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
{
    private readonly IValidator<T> _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arg = context.GetArgument<T>(0) ?? throw new InvalidOperationException(EM.VALIDATION_REQ_FIRSTARG);

        var result = await _validator.ValidateAsync(arg);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            var error = string.Join(", ", errors);

            return TypedResults.Problem(detail: error);
        }

        return await next(context);
    }
}
