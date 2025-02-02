using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalRightsManagement.Infrastructure.Persistence.EntityTypeConfigurations;

internal sealed class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.Username)
            .HasMaxLength(Username.MaxLength)
            .HasConversion(
                email => email.Value,
                s => Username.From(s));

        builder.Property(u => u.Email)
            .HasMaxLength(EmailAddress.MaxLength)
            .HasConversion(
                email => email.Value,
                s => EmailAddress.From(s));

        builder.Property(u => u.Role)
            .HasConversion<string>();

        builder.PrimitiveCollection(p => p.Products)
            .HasField("_products")
            .ElementType()
            .HasConversion<ProductIdConverter>();

        builder.Ignore(u => u.DomainEvents);
    }
}
