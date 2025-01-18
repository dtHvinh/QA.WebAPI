using System.Diagnostics;

namespace WebAPI.Utilities.Result.Base;

[DebuggerDisplay("IsSucceed={IsSuccess}")]
public class GenericResult
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; } = string.Empty;

    protected GenericResult() { }

    protected GenericResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static GenericResult Success(string message = "")
        => new(true, message);

    public static GenericResult Failure(string message)
        => new(false, message);
}

public class GenericResult<T> : GenericResult
{
    public T? Value { get; private set; }

    protected GenericResult() : base() { }

    protected GenericResult(T? value, bool isSuccess, string message) : base(isSuccess, message)
    {
        Value = value;
    }

    public static GenericResult<T> Success(T value, string message = "")
        => new(value, true, message);

    public new static GenericResult<T> Failure(string message)
        => new(default, false, message);
}
