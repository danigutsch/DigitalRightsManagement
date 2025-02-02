using Ardalis.Result;

namespace DigitalRightsManagement.Application.ProductAggregate;

public static class ValidationCombiner
{
    public static Result<(T1, T2)> Combine<T1, T2>(Result<T1> result1, Result<T2> result2)
    {
        ValidationError[] errors = [.. result1.ValidationErrors.Concat(result2.ValidationErrors)];
        return errors.Length > 0
            ? Result.Invalid(errors)
            : Result.Success();
    }

    public static Result<(T1, T2, T3)> Combine<T1, T2, T3>(Result<T1> result1, Result<T2> result2, Result<T3> result3)
    {
        ValidationError[] errors = [.. result1.ValidationErrors
            .Concat(result2.ValidationErrors)
            .Concat(result3.ValidationErrors)];

        return errors.Length > 0
            ? Result.Invalid(errors)
            : Result.Success();
    }
}
