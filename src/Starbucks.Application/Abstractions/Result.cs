using FluentValidation.Results;
using Starbucks.Domain.Abstractions;

namespace Starbucks.Application.Abstractions;

public class Result<T> : ResultGlobal
{
    public T? Value { get; init; }
    private Result(){}
    public static Result<T> Success(T value) => new Result<T>
    {
        IsSucces = true,
        Value = value
    };
    public static Result<T> Failure(params Error[] errors) => new Result<T>
    {
        IsSucces = false,
        Errors = errors.ToList()
    };
    public static Result<T> ValidationFailure(
        IEnumerable<ValidationFailure> failures)
        => Failure(
            failures
                .Select(f => new Error(f.PropertyName, f.ErrorMessage)).ToArray()
        );

}
public class Result :ResultGlobal
{
    public static Result Success() => new Result
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
    public bool IsFailure => !IsSucces;
    public List<Error> Errors { get; init; } = new();
    protected ResultGlobal() { }
}


