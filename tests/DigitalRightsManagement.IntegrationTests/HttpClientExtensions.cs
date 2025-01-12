using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.MigrationService;

namespace DigitalRightsManagement.IntegrationTests;
internal static class HttpClientExtensions
{
    public static void AddBasicAuth(this HttpClient httpClient, User user)
    {
        var basicAuthHeader = new BasicAuthenticationHeaderValue(user.Username, SeedData.Passwords[user.Id]);
        httpClient.DefaultRequestHeaders.Authorization = basicAuthHeader;
    }
}
