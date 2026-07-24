namespace TaskManagement.Utilites;

public class Error
{
    public ErrorType Type { get; }
    public string? ErrorTitle { get; }
    public string? ErrorDetail { get; }

    private Error(ErrorType errorType, string? errorTitle = null, string? errorDetail = null)
    {
        Type = errorType;
        ErrorTitle = errorTitle;
        ErrorDetail = errorDetail;
    }
    public static Error NotFound(string? detail = null) =>
     new(ErrorType.NotFound, errorTitle: "Resource Not Found", errorDetail: detail);

    public static Error BadRequest(string? detail = null) =>
        new(ErrorType.BadRequest, errorTitle: "BadRequest", errorDetail: detail);

    public static Error Conflict(string? detail = null) =>
        new(ErrorType.Conflict, errorTitle: "Conflict", errorDetail: detail);

    public static Error Unauthorized(string? detail = null) =>
        new(ErrorType.Unauthorized, errorTitle: "Unauthorized", errorDetail: detail);

    public static Error Forbidden(string? detail = null) =>
        new(ErrorType.Forbidden, errorTitle: "Forbidden", errorDetail: detail);

    public static Error InternalServerError(string? detail = null) =>
        new(ErrorType.InternalError, errorTitle: "Internal Server Error", errorDetail: detail);

}