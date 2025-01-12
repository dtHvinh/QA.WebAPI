using FluentValidation;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Filters.Validation;

public class FluentValidation<T>(IValidator<T> validator) : IEndpointFilter
{
    private readonly IValidator<T> _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arg = context.GetArgument<T>(0) ?? throw new InvalidOperationException(EM.VALIDATION_REQ_FIRSTARG);

        var result = await _validator.ValidateAsync(arg);

        if (!result.IsValid)
        {
            return ProblemResultExtensions.ValidationProblem(result.Errors);
        }

        return await next(context);
    }
}
