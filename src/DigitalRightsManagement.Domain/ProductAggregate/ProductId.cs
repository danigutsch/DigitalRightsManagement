using Ardalis.Result;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public readonly record struct ProductId
{
    public Guid Value { get; }

    private ProductId(Guid value) => Value = value;

    public static ProductId Create() => new(Guid.CreateVersion7());

    public static ProductId From(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be empty.");
        }

        return new ProductId(id);
    }

    public static Result<ProductId> From(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Errors.Products.EmptyId();
        }

        if (!Guid.TryParse(id, out var guid))
        {
            return Errors.Products.InvalidId(id);
        }

        return From(guid);
    }

    public override string ToString() => Value.ToString();
}