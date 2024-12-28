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

        builder.Property(p => p.Name);

        builder.Property(p => p.Description);

        builder.OwnsOne(p => p.Price);

        builder.Property(p => p.CreatedBy);

        builder.Property(p => p.Status)
            .HasConversion<string>();

        builder.Ignore(p => p.DomainEvents);
    }
}
