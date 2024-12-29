using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Domain;

internal static class Errors
{
    internal static class Product
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

        public static Result EmptyCreatorId()
        {
            const string code = "product.created-by.empty";
            const string message = "The creator ID can not be empty.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result InvalidStatusChange(ProductStatus currentStatus, ProductStatus desiredStatus)
        {
            const string code = "product.status.invalid-change";
            var message = $"Invalid status change from {currentStatus} to {desiredStatus}.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result AlreadyInStatus(ProductStatus status)
        {
            const string code = "product.status.already-in-status";
            var message = $"The product is already in status {status}.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result InvalidManager(Guid userId, Guid manager)
        {
            const string code = "product.manager.invalid";
            var message = $"The user [{userId}] is not the manager of the product. Manager: [{manager}].";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result EmptyId()
        {
            const string code = "product.id.empty";
            const string message = "The product ID can not be empty.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }
    }

    internal static class User
    {
        public static Result EmptyId()
        {
            const string code = "user.id.empty";
            const string message = "The user ID can not be empty.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result InvalidUsername()
        {
            const string code = "user.username.invalid";
            const string message = "Invalid username.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result InvalidEmail()
        {
            const string code = "user.email.invalid";
            const string message = "Invalid email.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result UnknownRole()
        {
            const string code = "user.role.unknown";
            const string message = "Unknown role.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result UnauthorizedToPromote(Guid promoterId, Guid promoteeId, UserRoles desiredRole)
        {
            const string code = "user.unauthorized.promote";
            var message = $"User [{promoterId}] can not promote [{promoteeId}] to role {desiredRole}.";
            return Result.Unauthorized(code, message);
        }

        public static Result AlreadyInRole(Guid userId, UserRoles desiredRole)
        {
            const string code = "user.role.already-in-role";
            var message = $"The user [{userId}] is already in role {desiredRole}.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }
    }
}
