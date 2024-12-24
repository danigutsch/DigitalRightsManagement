using Ardalis.Result;

namespace DigitalRightsManagement.Common;

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
}
