using Microsoft.AspNetCore.Authorization;

namespace DigitalRightsManagement.Infrastructure.Authorization;

public static class Policies
{
    public const string IsAdmin = "IsAdmin";
    public const string IsManager = "IsManager";

    public static AuthorizationBuilder AddDefaultPolicies(this AuthorizationBuilder builder)
    {
        builder.AddPolicy(IsAdmin, policy => policy.RequireRole(AuthorizationRoles.Admin));
        builder.AddPolicy(IsManager, policy => policy.RequireRole(AuthorizationRoles.Manager));
        return builder;
    }
}
