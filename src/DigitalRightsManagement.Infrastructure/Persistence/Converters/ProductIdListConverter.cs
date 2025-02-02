using DigitalRightsManagement.Domain.ProductAggregate;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalRightsManagement.Infrastructure.Persistence.Converters;

internal sealed class ProductIdListConverter()
    : ValueConverter<List<ProductId>, IEnumerable<Guid>>(
        ids => ids.Select(id => id.Value),
        ids => ids.Select(ProductId.From).ToList());


internal sealed class ProductIdConverter()
    : ValueConverter<ProductId, Guid>(
        id => id.Value,
        id => ProductId.From(id));
