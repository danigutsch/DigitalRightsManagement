using Microsoft.AspNetCore.Authentication;

namespace DigitalRightsManagement.Infrastructure.Authentication;

public static class AuthenticationDependencyInjection
{
    public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationDefaults.AuthenticationScheme, null);
    }
}
