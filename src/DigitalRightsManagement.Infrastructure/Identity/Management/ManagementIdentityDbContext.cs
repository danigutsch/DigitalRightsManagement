using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Identity.Management;

internal sealed class ManagementIdentityDbContext(DbContextOptions<ManagementIdentityDbContext> options) : DomainIdentityDbContext<ManagementIdentityUser>(options)
{
    protected override string SchemaName => "management_identity";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(SchemaName);
    }
}
