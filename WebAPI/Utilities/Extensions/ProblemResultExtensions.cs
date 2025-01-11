using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAPI.Utilities.Extensions;

public static class ProblemResultExtensions
{
    public static ProblemHttpResult ValidationProblem(List<ValidationFailure> failures)
    {
        return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest,
                                    title: "Validation Error",
                                    extensions: new Dictionary<string, object?>()
                                    {
                                        {"errors", failures.ToDictionary(e => e.PropertyName, e => e.ErrorMessage) }
                                    });
    }

    public static ProblemHttpResult BadRequest(string message)
    {
        return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest,
                                    title: "Error",
                                    extensions: new Dictionary<string, object?>()
                                    {
                                        {"errors", message }
                                    });
    }
}
