namespace Enjaz.SharedKernel.Results;

public sealed class Result<T> : Result
{
    private Result(T? value, bool isSuccess, string? errorCode, string? errorMessage)
        : base(isSuccess, errorCode, errorMessage)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(value, true, null, null);

    public static new Result<T> Failure(string code, string message) => new(default, false, code, message);
}
