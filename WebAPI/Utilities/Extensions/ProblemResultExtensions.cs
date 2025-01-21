using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAPI.Utilities.Extensions;

public static class ProblemResultExtensions
{
    public static ProblemHttpResult ValidationProblem(List<ValidationFailure> failures)
    {
        return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest,
                                    title: failures.FirstOrDefault()?.ErrorMessage ?? "Error",
                                    extensions: new Dictionary<string, object?>()
                                    {
                                        {"errors", failures.ToDictionary(e => e.PropertyName, e => e.ErrorMessage) }
                                    });
    }

    public static ProblemHttpResult BadRequest(string message)
    {
        return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest,
                                    title: message,
                                    extensions: new Dictionary<string, object?>()
                                    {
                                        {"errors", message }
                                    });
    }

    public static ProblemHttpResult NotFound(string message)
    {
        return TypedResults.Problem(statusCode: StatusCodes.Status404NotFound,
                                    title: message,
                                    extensions: new Dictionary<string, object?>()
                                    {
                                        {"errors", message }
                                    });
    }

    public static ProblemHttpResult Forbidden(string message)
    {
        return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden,
                                    title: message,
                                    extensions: new Dictionary<string, object?>()
                                    {
                                        {"errors", message }
                                    });
    }
}
