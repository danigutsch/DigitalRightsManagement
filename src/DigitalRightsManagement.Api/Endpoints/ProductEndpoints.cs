using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Application.UserAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalRightsManagement.Api.Endpoints;

internal static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products");

        group.MapGet("/", GetProducts)
            .WithName("Get Products")
            .WithSummary("Get the projects of a user")
            .WithDescription("Allows a user to get the products he/she manages.")
            .Produces<ProductDto[]>()
            .RequireAuthorization(Policies.IsManager);

        group.MapPost("/create", Create)
            .WithName("Create Product")
            .WithSummary("Create a new product")
            .WithDescription("Allows a manager or higher to create a new product.")
            .ProducesDefault()
            .RequireAuthorization(Policies.IsManager);
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
}

public sealed record CreateProductDto(string ProductName, string ProductDescription, decimal Price, Currency Currency);
