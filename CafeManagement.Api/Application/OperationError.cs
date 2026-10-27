namespace CafeManagement.Api.Application;

public enum OperationErrorKind
{
    Unauthenticated,
    Forbidden,
    NotFound,
    Invalid,
    Conflict,
    Unexpected
}

// Messages are safe application feedback, never raw exception or database text.
public sealed record OperationError(
    OperationErrorKind Kind,
    string Message,
    string? Code = null,
    string? Field = null,
    IReadOnlyDictionary<string, string[]>? FieldErrors = null);
