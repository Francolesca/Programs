using System;

namespace Starbucks.Application.Abstractions;

public class Result<T> : ResultGlobal
{
    public T? Value { get; init; }
    public static Result<T> Succes(T value) => new Result<T>
    {
        IsSucces = true,
        Value = value
    };
    public static Result<T> Failure(params Error[] errors) => new Result<T>
    {
        IsSucces = false,
        Errors = errors.ToList()
    };

}
public class Result :ResultGlobal
{
    public static Result Succes() => new Result
    {
        IsSucces = true,
    };
    public static Result Failure(params Error[] errors) => new Result
    {
        IsSucces = false,
        Errors = errors.ToList()
    };
}

public abstract class ResultGlobal
{
    public bool IsSucces { get; init; }
    public bool IsFailure { get; init; }
    public List<Error> Errors { get; init; } = new();
    protected ResultGlobal() { }
}


