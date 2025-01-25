namespace DigitalRightsManagement.IntegrationTests.Helpers.HttpAuthHandlers;

internal sealed class BasicAuthHandler(string username, string password) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new BasicAuthenticationHeaderValue(username, password);
        return await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
