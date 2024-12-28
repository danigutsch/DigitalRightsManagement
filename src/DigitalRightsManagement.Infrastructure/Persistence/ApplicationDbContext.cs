using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Persistence.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();

    public async Task SaveChanges(CancellationToken cancellationToken) => await SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}
