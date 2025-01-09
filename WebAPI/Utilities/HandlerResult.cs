namespace WebAPI.Utilities;

public class HandlerResult
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; } = string.Empty;

    protected HandlerResult() { }

    protected HandlerResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static HandlerResult Success(string message = "")
        => new(true, message);

    public static HandlerResult Failure(string message)
        => new(false, message);
}

public class HandlerResult<T> : HandlerResult
{
    public T? Value { get; private set; }

    private HandlerResult() : base() { }

    private HandlerResult(T? value, bool isSuccess, string message) : base(isSuccess, message)
    {
        Value = value;
    }

    public static HandlerResult<T> Success(T value, string message = "")
        => new(value, true, message);

    public new static HandlerResult<T> Failure(string message)
        => new(default, false, message);
}
