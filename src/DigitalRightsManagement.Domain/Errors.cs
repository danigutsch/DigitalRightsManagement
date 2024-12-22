using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Domain;

internal static class Errors
{
    internal static class Product
    {
        internal static class Create
        {
            public static Result InvalidName()
            {
                const string code = "product.name.invalid";
                const string message = "Invalid name.";
                return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
            }

            public static Result InvalidDescription()
            {
                const string code = "product.description.invalid";
                const string message = "Invalid description.";
                return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
            }

            public static Result InvalidPrice(decimal price)
            {
                const string code = "product.price.negative-value";
                var message = $"The price can not be negative. Was: {price:C}.";
                return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
            }

            public static Result InvalidCurrency(Currency currency)
            {
                const string code = "product.price.unknown-currency";
                var message = $"Unknown currency: {currency}.";
                return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
            }

            public static Result EmptyId()
            {
                const string code = "product.created-by.empty";
                const string message = "The creator ID can not be empty.";
                return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
            }
        }
    }
}
