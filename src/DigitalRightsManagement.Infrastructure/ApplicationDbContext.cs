using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure;

internal sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    public async Task SaveChanges(CancellationToken cancellationToken) => await SaveChangesAsync(cancellationToken);
}
