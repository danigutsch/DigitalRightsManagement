#pragma warning disable CA1034

using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Domain;

public static class Errors
{
    public static class Products
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
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Warning));
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

        public static Result AlreadyOwned(Guid userId, Guid productId)
        {
            const string code = "product.already-owned";
            var message = $"The user [{userId}] already owns the product [{productId}].";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Warning));
        }

        public static Result AlreadyOwned(Guid userId)
        {
            const string code = "product.already-owned";
            var message = $"The user [{userId}] already owns all products.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Warning));
        }

        public static Result NotFound(Guid productId)
        {
            const string code = "product.not-found";
            var message = $"The product [{productId}] was not found.";
            return Result.NotFound(code, message);
        }
    }

    public static class Users
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
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Warning));
        }

        public static Result UnauthorizedToOwnProduct(Guid id)
        {
            const string code = "user.unauthorized.own-product";
            var message = $"The can not [{id}] own a product.";
            return Result.Unauthorized(code, message);
        }

        public static Result NotFound()
        {
            const string code = "user.not-found";
            const string message = "The user was not found.";
            return Result.NotFound(code, message);
        }

        public static Result Unauthorized(UserRoles requiredRole)
        {
            const string code = "user.unauthorized";
            var message = $"The user is not authorized to perform this action. Required role: {requiredRole}.";
            return Result.Unauthorized(code, message);
        }
    }
}
