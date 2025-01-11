using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence.Identity;

internal sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<AuthUser>(options)
{
    private const string SchemaName = "identity";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(SchemaName);

        builder.Entity<AuthUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(user => user.UserName).IsRequired();
            entity.Property(user => user.PasswordHash).IsRequired();
        });

        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

    }
}
