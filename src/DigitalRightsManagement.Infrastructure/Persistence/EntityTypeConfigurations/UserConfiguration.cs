using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalRightsManagement.Infrastructure.Persistence.EntityTypeConfigurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.Username);

        builder.Property(u => u.Email);

        builder.Property(u => u.Role)
            .HasConversion<string>();

        builder.Ignore(u => u.DomainEvents);
    }
}
