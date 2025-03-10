﻿using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;

namespace DigitalRightsManagement.Common.DDD;

public static class ResultExtensions
{
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

    /// <summary>
    /// Asynchronously executes a function if the result is successful.
    /// </summary>
    /// <param name="resultTask">The task representing the result.</param>
    /// <param name="action">The function to execute if the result is successful.</param>
    /// <returns>A task representing the original result.</returns>
    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> resultTask, Func<T, Task> action)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            await action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute if the result is successful.</param>
    /// <returns>The original result.</returns>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Asynchronously executes a function if the result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The function to execute if the result is successful.</param>
    /// <returns>A task representing the original result.</returns>
    public static async Task<Result<T>> Tap<T>(this Result<T> result, Func<T, Task> action)
    {
        if (result.IsSuccess)
        {
            await action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Asynchronously binds a function to the result.
    /// </summary>
    /// <typeparam name="TSource">The type of the result value.</typeparam>
    /// <param name="resultTask">The task representing the result.</param>
    /// <param name="bindFunc">The function to bind to the result.</param>
    /// <returns>A task representing the new result.</returns>
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

    /// <summary>
    /// Handles non-success statuses and returns the appropriate result.
    /// </summary>
    /// <typeparam name="TSource">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>The appropriate result based on the status.</returns>
    private static Result HandleNonSuccessStatus<TSource>(Result<TSource> result)
    {
        return result.Status switch
        {
            ResultStatus.NotFound => result.Errors.Any()
                ? Result.NotFound([.. result.Errors])
                : Result.NotFound(),
            ResultStatus.Unauthorized => result.Errors.Any()
                ? Result.Unauthorized([.. result.Errors])
                : Result.Unauthorized(),
            ResultStatus.Forbidden => result.Errors.Any()
                ? Result.Forbidden([.. result.Errors])
                : Result.Forbidden(),
            ResultStatus.Invalid => Result.Invalid(result.ValidationErrors),
            ResultStatus.Error => Result.Error(
                new ErrorList([.. result.Errors], result.CorrelationId)
            ),
            ResultStatus.Conflict => result.Errors.Any()
                ? Result.Conflict([.. result.Errors])
                : Result.Conflict(),
            ResultStatus.CriticalError => Result.CriticalError([.. result.Errors]),
            ResultStatus.Unavailable => Result.Unavailable([.. result.Errors]),
            ResultStatus.NoContent => Result.NoContent(),
            _ => throw new NotSupportedException(
                $"Result {result.Status} conversion is not supported."
            ),
        };
    }

    public static bool TryGetValue<T>(this Result<T> result, [NotNullWhen(true)] out T? value)
    {
        if (result.IsSuccess)
        {
            value = result.Value!;
            return true;
        }

        value = default;
        return false;
    }
}
