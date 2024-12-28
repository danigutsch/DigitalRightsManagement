using Ardalis.Result.AspNetCore;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Api;

internal static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products");

        group.MapPost("/create", Create)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .WithDescription("Allows a manager or higher to create a new product.")
            .ProducesDefault();
    }

    private static async Task<IResult> Create(CreateProductDto dto, CreateProductCommandHandler handler, CancellationToken ct)
    {
        var command = new CreateProductCommand(dto.UserId, dto.ProductName, dto.ProductDescription, dto.Price, dto.Currency);
        var result = await handler.Handle(command, ct);

        return result.ToMinimalApiResult();
    }
}

public sealed record CreateProductDto(Guid UserId, string ProductName, string ProductDescription, decimal Price, Currency Currency);
