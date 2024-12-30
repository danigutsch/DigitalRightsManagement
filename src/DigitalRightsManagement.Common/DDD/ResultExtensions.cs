using Ardalis.Result;
using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Common.DDD;

public static class ResultExtensions
{
    /// <summary>
    /// Binds a function to the result and returns a new result containing a tuple of the previous and next values.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous result value.</typeparam>
    /// <typeparam name="TNext">The type of the next result value.</typeparam>
    /// <param name="previousResult">The previous result.</param>
    /// <param name="bindFunc">The function to bind to the result.</param>
    /// <returns>A new result containing a tuple of the previous and next values.</returns>
    public static Result<(TPrevious, TNext)> DoubleBind<TPrevious, TNext>(this Result<TPrevious> previousResult, Func<TPrevious, Result<TNext>> bindFunc)
    {
        return previousResult.Bind(bindFunc).Map(next => (previousResult.Value, next));
    }

    /// <summary>
    /// Asynchronously binds a function to the result and returns a new result containing a tuple of the previous and next values.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous result value.</typeparam>
    /// <typeparam name="TNext">The type of the next result value.</typeparam>
    /// <param name="previousResultTask">The task representing the previous result.</param>
    /// <param name="bindFunc">The function to bind to the result.</param>
    /// <returns>A task representing the new result containing a tuple of the previous and next values.</returns>
    public static Task<Result<(TPrevious Prev, TNext Next)>> DoubleBind<TPrevious, TNext>(this Task<Result<TPrevious>> previousResultTask, Func<TPrevious, Task<Result<TNext>>> bindFunc)
    {
        return previousResultTask.BindAsync(bindFunc).MapAsync(async next => ((await previousResultTask).Value, next));
    }

    /// <summary>
    /// Asynchronously binds a function to the result and returns a new result containing a tuple of the previous and next values.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous result value.</typeparam>
    /// <typeparam name="TNext">The type of the next result value.</typeparam>
    /// <param name="previousResultTask">The task representing the previous result.</param>
    /// <param name="bindFunc">The function to bind to the result.</param>
    /// <returns>A task representing the new result containing a tuple of the previous and next values.</returns>
    public static Task<Result<(TPrevious Prev, TNext Next)>> DoubleBind<TPrevious, TNext>(this Task<Result<TPrevious>> previousResultTask, Func<TPrevious, Result<TNext>> bindFunc)
    {
        return previousResultTask.BindAsync(bindFunc).MapAsync(async next => ((await previousResultTask).Value, next));
    }

    /// <summary>
    /// Asynchronously executes a function if the result is successful.
    /// </summary>
    /// <param name="resultTask">The task representing the result.</param>
    /// <param name="task">The function to execute if the result is successful.</param>
    /// <returns>A task representing the original result.</returns>
    public static async Task<Result> Tap(this Task<Result> resultTask, Func<Task> task)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            await task();
        }

        return result;
    }

    /// <summary>
    /// Asynchronously executes a function if the result is successful.
    /// </summary>
    /// <param name="resultTask">The task representing the result.</param>
    /// <param name="action">The function to execute if the result is successful.</param>
    /// <returns>A task representing the original result.</returns>
    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> resultTask, Action<T> action)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    public static async Task<Result> BindAsync<TSource>(
        this Task<Result<TSource>> resultTask,
        Func<TSource, Result> bindFunc
    )
    {
        var result = await resultTask;
        return result.Status switch
        {
            ResultStatus.Ok => bindFunc(result.Value),
            ResultStatus.Created => bindFunc(result.Value),
            _ => HandleNonSuccessStatus(result),
        };
    }

    private static Result HandleNonSuccessStatus<TSource>(Result<TSource> result)
    {
        return result.Status switch
        {
            ResultStatus.NotFound => result.Errors.Any()
                ? Result.NotFound(result.Errors.ToArray())
                : Result.NotFound(),
            ResultStatus.Unauthorized => result.Errors.Any()
                ? Result.Unauthorized(result.Errors.ToArray())
                : Result.Unauthorized(),
            ResultStatus.Forbidden => result.Errors.Any()
                ? Result.Forbidden(result.Errors.ToArray())
                : Result.Forbidden(),
            ResultStatus.Invalid => Result.Invalid(result.ValidationErrors),
            ResultStatus.Error => Result.Error(
                new ErrorList(result.Errors.ToArray(), result.CorrelationId)
            ),
            ResultStatus.Conflict => result.Errors.Any()
                ? Result.Conflict(result.Errors.ToArray())
                : Result.Conflict(),
            ResultStatus.CriticalError => Result.CriticalError(result.Errors.ToArray()),
            ResultStatus.Unavailable => Result.Unavailable(result.Errors.ToArray()),
            ResultStatus.NoContent => Result.NoContent(),
            _ => throw new NotSupportedException(
                $"Result {result.Status} conversion is not supported."
            ),
        };
    }
}
