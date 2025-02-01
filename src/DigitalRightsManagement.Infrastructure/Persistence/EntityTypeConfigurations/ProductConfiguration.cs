using DigitalRightsManagement.Domain.ProductAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalRightsManagement.Infrastructure.Persistence.EntityTypeConfigurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasConversion(
                name => name.Value,
                s => ProductName.From(s))
            .HasMaxLength(ProductName.MaxLength);

        builder.Property(p => p.Description)
            .HasConversion(
                description => description.Value,
                s => Description.From(s))
            .HasMaxLength(Description.MaxLength);

        builder.ComplexProperty(
            p => p.Price,
            p =>
            {
                p.Property(x => x.Amount)
                    .HasColumnName("Price_Amount");

                p.Property(x => x.Currency)
                    .HasColumnName("Price_Currency");
            });

        builder.Property(p => p.AgentId);

        builder.Property(p => p.Status)
            .HasConversion<string>();

        builder.PrimitiveCollection(p => p.AssignedWorkers)
            .HasField("_assignedWorkers");

        builder.Ignore(p => p.DomainEvents);
    }
}
