using Scalar.AspNetCore;

namespace DigitalRightsManagement.Api;

internal static class OpenApiExtensions
{
    public static WebApplication UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = ApiConstants.Name;
                options.Servers = [];
            });
        }

        return app;
    }
}
