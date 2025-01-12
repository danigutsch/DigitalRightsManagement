using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace DigitalRightsManagement.Infrastructure.Authentication;

public static class BasicAuthenticationDefaults
{
    public const string AuthenticationScheme = "Basic";

    public static readonly OpenApiSecurityScheme SecurityScheme = new()
    {
        Type = SecuritySchemeType.Http,
        Name = HeaderNames.Authorization,
        Scheme = AuthenticationScheme,
        Description = "Basic authentication header",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = AuthenticationScheme
        }
    };
}
