namespace DigitalRightsManagement.Infrastructure.Authentication;

public struct BasicAuthenticationCredentials(string username, string password)
{
    public string Username { get; } = username;
    public string Password { get; set; } = password;
}
