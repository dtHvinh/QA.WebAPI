namespace WebAPI.Utilities;

/// <summary>
/// Represents the result of a query operation.
/// </summary>
public class QueryResult
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; } = string.Empty;

    protected QueryResult() { }

    protected QueryResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static QueryResult Success(string message = "")
        => new(true, message);

    public static QueryResult Failure(string message)
        => new(false, message);
}

/// <summary>
/// Represents the result of a query operation with a value.
/// </summary>
/// <typeparam name="T">The type.</typeparam>
public class QueryResult<T> : QueryResult
{
    public T? Value { get; private set; }

    private QueryResult() : base() { }

    private QueryResult(T? value, bool isSuccess, string message) : base(isSuccess, message)
    {
        Value = value;
    }

    public static QueryResult<T> Success(T value, string message = "")
        => new(value, true, message);

    public new static QueryResult<T> Failure(string message)
        => new(default, false, message);
}


