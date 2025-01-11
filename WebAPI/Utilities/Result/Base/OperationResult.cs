namespace WebAPI.Utilities.Result.Base;

public class ResultBase
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; } = string.Empty;

    protected ResultBase() { }

    protected ResultBase(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static ResultBase Success(string message = "")
        => new(true, message);

    public static ResultBase Failure(string message)
        => new(false, message);
}

public class OperationResult<T> : ResultBase
{
    public T? Value { get; private set; }

    protected OperationResult() : base() { }

    protected OperationResult(T? value, bool isSuccess, string message) : base(isSuccess, message)
    {
        Value = value;
    }

    public static OperationResult<T> Success(T value, string message = "")
        => new(value, true, message);

    public new static OperationResult<T> Failure(string message)
        => new(default, false, message);
}
