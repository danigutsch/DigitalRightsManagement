#pragma warning disable CA1034

using Ardalis.Result;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;

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

        public static Result InvalidManager(Guid agentId, Guid manager)
        {
            const string code = "product.manager.invalid";
            var message = $"The agent [{agentId}] is not the manager of the product. Manager: [{manager}].";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result EmptyId()
        {
            const string code = "product.id.empty";
            const string message = "The product ID can not be empty.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result AlreadyOwned(Guid agentId, Guid productId)
        {
            const string code = "product.already-owned";
            var message = $"The agent [{agentId}] already owns the product [{productId}].";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Warning));
        }

        public static Result AlreadyOwned(Guid agentId)
        {
            const string code = "product.already-owned";
            var message = $"The agent [{agentId}] already owns all products.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Warning));
        }

        public static Result NotFound(Guid productId)
        {
            const string code = "product.not-found";
            var message = $"The product [{productId}] was not found.";
            return Result.NotFound(code, message);
        }
    }

    public static class Agents
    {
        public static Result EmptyId()
        {
            const string code = "agent.id.empty";
            const string message = "The agent ID can not be empty.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result InvalidUsername()
        {
            const string code = "agent.username.invalid";
            const string message = "Invalid username.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result InvalidEmail()
        {
            const string code = "agent.email.invalid";
            const string message = "Invalid email.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result UnknownRole()
        {
            const string code = "agent.role.unknown";
            const string message = "Unknown role.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Error));
        }

        public static Result UnauthorizedToPromote(Guid promoterId, Guid promoteeId, AgentRoles desiredRole)
        {
            const string code = "agent.unauthorized.promote";
            var message = $"Agent [{promoterId}] can not promote [{promoteeId}] to role {desiredRole}.";
            return Result.Unauthorized(code, message);
        }

        public static Result AlreadyInRole(Guid agentId, AgentRoles desiredRole)
        {
            const string code = "agent.role.already-in-role";
            var message = $"The agent [{agentId}] is already in role {desiredRole}.";
            return Result.Invalid(new ValidationError(code, message, code, ValidationSeverity.Warning));
        }

        public static Result UnauthorizedToOwnProduct(Guid id)
        {
            const string code = "agent.unauthorized.own-product";
            var message = $"The can not [{id}] own a product.";
            return Result.Unauthorized(code, message);
        }

        public static Result NotFound()
        {
            const string code = "agent.not-found";
            const string message = "The agent was not found.";
            return Result.NotFound(code, message);
        }

        public static Result Unauthorized(AgentRoles requiredRole)
        {
            const string code = "agent.unauthorized";
            var message = $"The agent is not authorized to perform this action. Required role: {requiredRole}.";
            return Result.Unauthorized(code, message);
        }
    }

    public static class Identity
    {
        public static Result InvalidAuthCredentials()
        {
            const string code = "identity.credentials.missing";
            const string message = "Missing or invalid authentication credentials";
            return Result.Invalid(new ValidationError(code, message));
        }

        public static Result InvalidClaim()
        {
            const string code = "identity.claim.invalid";
            const string message = "Invalid or missing identity claim";
            return Result.Invalid(new ValidationError(code, message));
        }
    }
}
