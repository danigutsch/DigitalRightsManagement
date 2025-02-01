using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using Validated = (DigitalRightsManagement.Domain.ProductAggregate.ProductName Name, DigitalRightsManagement.Domain.ProductAggregate.Description Description, DigitalRightsManagement.Domain.ProductAggregate.Price Price);

namespace DigitalRightsManagement.Application.ProductAggregate;

[Authorize(AgentRoles.Manager)]
public sealed record CreateProductCommand(string Name, string Description, decimal PriceAmount, Currency Currency) : ICommand<Guid>
{
    internal sealed class CreateProductCommandHandler(ICurrentAgentProvider currentAgentProvider, IProductRepository productRepository) : ICommandHandler<CreateProductCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var validationResult = Validate(command);
            if (!validationResult.TryGetValue(out var validated))
            {
                return validationResult.Map();
            }

            var agentResult = await currentAgentProvider.Get(cancellationToken);
            if (!agentResult.TryGetValue(out var agent))
            {
                return agentResult.Map();
            }

            return await Product.Create(validated.Name, validated.Description, validated.Price, agent.Id)
                .Tap(productRepository.Add)
                .Tap(_ => productRepository.UnitOfWork.SaveEntities(cancellationToken))
                .MapAsync(product => product.Id);
        }

        private static Result<Validated> Validate(CreateProductCommand command)
        {
            var name = ProductName.From(command.Name);
            var description = Domain.ProductAggregate.Description.From(command.Description);
            var price = Price.Create(command.PriceAmount, command.Currency);

            var combined = ValidationCombiner.Combine(name, description, price);

            if (!combined.IsSuccess)
            {
                return combined;
            }

            return (name.Value, description.Value, price.Value);
        }
    }
}

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
