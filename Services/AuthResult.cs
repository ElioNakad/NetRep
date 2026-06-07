namespace MyAzureDemo.Services;

public sealed class AuthResult<T>
{
    private AuthResult(bool succeeded, T? value, IEnumerable<object>? errors, int statusCode)
    {
        Succeeded = succeeded;
        Value = value;
        Errors = errors ?? [];
        StatusCode = statusCode;
    }

    public bool Succeeded { get; }

    public T? Value { get; }

    public IEnumerable<object> Errors { get; }

    public int StatusCode { get; }

    public static AuthResult<T> Success(T value)
    {
        return new AuthResult<T>(true, value, null, StatusCodes.Status200OK);
    }

    public static AuthResult<T> Failure(IEnumerable<object> errors, int statusCode = StatusCodes.Status400BadRequest)
    {
        return new AuthResult<T>(false, default, errors, statusCode);
    }
}
