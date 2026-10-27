using CafeManagement.Api.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CafeManagement.Api.Contracts;

public static class ApiErrors
{
    public static ProblemDetails CreateProblem(OperationError error)
    {
        var status = error.Kind switch
        {
            OperationErrorKind.Unauthenticated => StatusCodes.Status401Unauthorized,
            OperationErrorKind.Forbidden => StatusCodes.Status403Forbidden,
            OperationErrorKind.NotFound => StatusCodes.Status404NotFound,
            OperationErrorKind.Invalid => StatusCodes.Status400BadRequest,
            OperationErrorKind.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        ProblemDetails problem = error.Kind == OperationErrorKind.Invalid
            ? new ValidationProblemDetails(error.FieldErrors is null
                ? new Dictionary<string, string[]>()
                : new Dictionary<string, string[]>(error.FieldErrors))
            : new ProblemDetails();
        problem.Status = status;
        problem.Title = ReasonPhrases.GetReasonPhrase(status);
        problem.Type = "about:blank";
        problem.Detail = status == 500 ? "An unexpected error occurred. Please try again later." : error.Message;
        if (status != 500)
        {
            if (error.Code is not null) problem.Extensions["errorCode"] = error.Code;
            if (error.Field is not null) problem.Extensions["field"] = error.Field;
        }

        return problem;
    }

    public static ObjectResult ToResult(OperationError error)
    {
        var problem = CreateProblem(error);
        var result = new ObjectResult(problem) { StatusCode = problem.Status };
        result.ContentTypes.Add("application/problem+json");
        return result;
    }

    public static IActionResult InvalidModelState(ActionContext context)
    {
        var fields = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(entry => entry.Key, _ => new[] { "The supplied value is invalid." });
        return ToResult(new OperationError(OperationErrorKind.Invalid,
            "Check the supplied values.", FieldErrors: fields));
    }

    public static ProblemDetails ForStatus(int status) => new()
    {
        Status = status,
        Title = ReasonPhrases.GetReasonPhrase(status),
        Type = "about:blank",
        Detail = status >= 500 ? "An unexpected error occurred. Please try again later." : null
    };
}
