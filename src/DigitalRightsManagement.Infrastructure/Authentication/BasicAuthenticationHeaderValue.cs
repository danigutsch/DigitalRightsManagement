using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;

namespace DigitalRightsManagement.Infrastructure.Authentication;

public sealed class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
{
    private BasicAuthenticationHeaderValue(string username, string password) : base(BasicAuthenticationDefaults.AuthenticationScheme, GetParameter(username, password))
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        Credentials = new BasicAuthenticationCredentials(username, password);
    }

    public BasicAuthenticationCredentials Credentials { get; }

    private new static BasicAuthenticationHeaderValue Parse(string input)
    {
        var authHeader = AuthenticationHeaderValue.Parse(input);
        if (authHeader.Scheme != BasicAuthenticationDefaults.AuthenticationScheme)
        {
            throw new FormatException("Invalid Authorization Scheme");
        }

        if (!TryParseCredentials(authHeader.Parameter, out var username, out var password))
        {
            throw new FormatException("Invalid Authorization Header");
        }

        return new BasicAuthenticationHeaderValue(username, password);
    }

    public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out BasicAuthenticationHeaderValue? parsedValue)
    {
        if (input is null)
        {
            parsedValue = null;
            return false;
        }

        try
        {
            parsedValue = Parse(input);
            return true;
        }
        catch (FormatException)
        {
            parsedValue = null;
            return false;
        }
        catch (DecoderFallbackException)
        {
            parsedValue = null;
            return false;
        }
    }

    private static bool TryParseCredentials(string? input, [NotNullWhen(true)] out string? username, [NotNullWhen(true)] out string? password)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            (username, password) = (null, null);
            return false;
        }

        var credentialBytes = Convert.FromBase64String(input);
        var credentials = Encoding.UTF8
            .GetString(credentialBytes)
            .Split(':', 2);

        if (credentials.Length != 2)
        {
            (username, password) = (null, null);
            return false;
        }

        if (string.IsNullOrWhiteSpace(credentials[0]) || string.IsNullOrWhiteSpace(credentials[1]))
        {
            (username, password) = (null, null);
            return false;
        }

        (username, password) = (credentials[0], credentials[1]);
        return true;
    }

    private static string GetParameter(string username, string password)
    {
        var credentials = $"{username}:{password}";
        var bytes = Encoding.UTF8.GetBytes(credentials);
        return Convert.ToBase64String(bytes);
    }
}
