using Ardalis.Result;

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

            public static Result InvalidPrice()
            {
                const string code = "product.price.invalid";
                const string message = "Price must be zero or larger.";
                return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
            }
        }
    }
}
