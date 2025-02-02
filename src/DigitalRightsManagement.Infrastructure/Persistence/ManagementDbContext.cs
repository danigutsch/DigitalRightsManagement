using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Infrastructure.Messaging;
using DigitalRightsManagement.Infrastructure.Persistence.Converters;
using DigitalRightsManagement.Infrastructure.Persistence.EntityTypeConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal sealed class ManagementDbContext(IPublisher publisher, DbContextOptions<ManagementDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new AgentConfiguration())
            .ApplyConfiguration(new ProductConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<List<ProductId>>().HaveConversion<ProductIdListConverter>();
        configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
        configurationBuilder.Properties<List<AgentId>>().HaveConversion<AgentIdListConverter>();
        configurationBuilder.Properties<AgentId>().HaveConversion<AgentIdConverter>();
    }

    public async Task SaveEntities(CancellationToken cancellationToken)
    {
        await SaveChangesAsync(cancellationToken);

        await publisher.PublishDomainEvents(this, cancellationToken);
    }
}
