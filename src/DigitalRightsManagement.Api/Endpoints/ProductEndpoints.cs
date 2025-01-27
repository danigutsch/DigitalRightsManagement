using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Infrastructure.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalRightsManagement.Api.Endpoints;

internal class ProductEndpoints : EndpointGroupBase
{
    protected override string GroupName => "Products";
    protected override string BaseRoute => "/products";

    protected override RouteGroupBuilder ConfigureEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", GetProducts)
            .WithName("Get Products")
            .WithSummary("Get the products of a agent")
            .WithDescription("Allows a agent to get the products he/she is responsible for.")
            .Produces<ProductDto[]>();

        group.MapPost("/create", Create)
            .WithName("Create Product")
            .WithSummary("Create a new product")
            .WithDescription("Allows a manager or higher to create a new product.")
            .ProducesDefault();

        group.MapPost("/{id:guid}/workers", AssignWorker)
            .WithName("Assign Worker")
            .WithSummary("Assign a worker to a product")
            .WithDescription("Allows a manager to assign a worker (user with Viewer role) to their product.")
            .ProducesDefault();

        group.MapDelete("/{id:guid}/workers/{workerId:guid}", UnassignWorker)
            .WithName("Unassign Worker")
            .WithSummary("Unassign a worker from a product")
            .WithDescription("Allows a manager to remove a worker from their product.")
            .ProducesDefault();

        group.MapPut("/{id:guid}/price", UpdatePrice)
            .WithName("Update Product Price")
            .WithSummary("Update a product's price")
            .WithDescription("Allows a manager to update the price of their product.")
            .ProducesDefault();

        group.MapPut("/{id:guid}/description", UpdateDescription)
            .WithName("Update Product Description")
            .WithSummary("Update a product's description")
            .WithDescription("Allows a manager to update the description of their product.")
            .ProducesDefault();

        group.MapPost("/{id:guid}/publish", PublishProduct)
            .WithName("Publish Product")
            .WithSummary("Publish a product")
            .WithDescription("Allows a manager to publish their product.")
            .ProducesDefault();

        group.MapPost("/{id:guid}/obsolete", MakeProductObsolete)
            .WithName("Obsolete Product")
            .WithSummary("Mark a product as obsolete")
            .WithDescription("Allows a manager to mark their product as obsolete.")
            .ProducesDefault();

        return group;
    }

    private static async Task<IResult> GetProducts([FromServices] IMediator mediator, CancellationToken ct)
    {
        var query = new GetProductsQuery();
        var result = await mediator.Send(query, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> Create([FromBody] CreateProductDto dto, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new CreateProductCommand(dto.ProductName, dto.ProductDescription, dto.Price, dto.Currency);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }
    private static async Task<IResult> AssignWorker(Guid id, [FromBody] AssignWorkerDto dto, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new AssignWorkerCommand(id, dto.WorkerId);
        var result = await mediator.Send(command, ct);
        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> UnassignWorker(Guid id, Guid workerId, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new UnassignWorkerCommand(id, workerId);
        var result = await mediator.Send(command, ct);
        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> UpdatePrice(Guid id, [FromBody] UpdatePriceDto dto, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new UpdatePriceCommand(id, dto.Price, dto.Currency, dto.Reason);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> UpdateDescription(Guid id, [FromBody] UpdateDescriptionDto dto, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new UpdateDescriptionCommand(id, dto.Description);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> PublishProduct(Guid id, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new PublishProductCommand(id);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> MakeProductObsolete(Guid id, [FromServices] IMediator mediator, CancellationToken ct)
    {
        var command = new MakeProductObsoleteCommand(id);
        var result = await mediator.Send(command, ct);

        return result.ToMinimalApiResult();
    }
}

public sealed record CreateProductDto(string ProductName, string ProductDescription, decimal Price, Currency Currency);

public sealed record AssignWorkerDto(Guid WorkerId);

public sealed record UpdatePriceDto(decimal Price, Currency Currency, string Reason);

public sealed record UpdateDescriptionDto(string Description);
