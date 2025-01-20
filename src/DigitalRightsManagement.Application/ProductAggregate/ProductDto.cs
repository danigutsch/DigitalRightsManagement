using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.ProductAggregate;

public sealed record ProductDto(string Name, string Description, decimal Price, Currency Currency);
