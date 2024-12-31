﻿using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Persistence.Converters;
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

        builder.Property(u => u.Username)
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .HasMaxLength(100);

        builder.Property(u => u.Role)
            .HasConversion<string>();

        builder.Property("_products")
            .HasColumnName("Products")
            .HasConversion(
                new ListOfGuidConverter(),
                new ListOfGuidComparer());

        builder.Ignore(u => u.DomainEvents);
    }
}
