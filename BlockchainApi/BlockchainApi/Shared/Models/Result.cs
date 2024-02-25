namespace Backend.Shared.Models;

public class Result
{
    public virtual object? Value { get; }
    private readonly bool _success;
    public virtual Error? Error { get; protected set; }
    public bool IsSuccess => _success;

    protected Result()
    {
        _success = true;
    }

    protected Result(Error error)
    {
        Error = error;
        _success = false;
    }

    public static Result Ok() => new();
    public static Result<T> Ok<T>(T val)
    {
        return Result<T>.Ok(val);
    }
    public static Result Fail(Error error) => new(error);

    public static implicit operator Result(Error e) => new(e);

    public R Match<R>(
    Func<R> success,
    Func<Error, R> failure) =>
        IsSuccess ? success() : failure(Error!);
}

public class Result<T> : Result
{
    public new T? Value { get; }

    private Result(T value) : base()
    {
        Value = value;
    }

    private Result(Error error) : base(error) { }

    public static Result<T> Ok(T val)
    {
        return new Result<T>(val);
    }


    public static new Result<T> Fail(Error error) => new(error);

    public static implicit operator Result<T>(T v) => new(v);

    public R Match<R>(
        Func<T, R> success,
        Func<Error, R> failure) =>
    IsSuccess ? success(Value!) : failure(Error!);

}
