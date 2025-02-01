using Ardalis.Result;

namespace DigitalRightsManagement.Application.ProductAggregate;

public static class ValidationCombiner
{
    public static Result Combine(params IResult[] results)
    {
        ValidationError[] errors = [.. results.SelectMany(result => result.ValidationErrors)];
        return errors.Length > 0
            ? Result.Invalid(errors)
            : Result.Success();
    }
}
