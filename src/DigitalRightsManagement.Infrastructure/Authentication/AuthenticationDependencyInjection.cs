using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace DigitalRightsManagement.Infrastructure.Authentication;

public static class AuthenticationDependencyInjection
{
    public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationDefaults.AuthenticationScheme, null);
    }

    public static OpenApiOptions AddBasic(this OpenApiOptions options) => AddSecurityScheme(options, BasicAuthenticationDefaults.SecurityScheme);

    private static OpenApiOptions AddSecurityScheme(OpenApiOptions options, OpenApiSecurityScheme securityScheme)
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes.Add(securityScheme.Scheme, securityScheme);
            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, _) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                operation.Security = [new OpenApiSecurityRequirement { [securityScheme] = [] }];
            }
            return Task.CompletedTask;
        });

        return options;
    }
}
