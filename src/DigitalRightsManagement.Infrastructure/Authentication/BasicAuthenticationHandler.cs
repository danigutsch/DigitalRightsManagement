using DigitalRightsManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DigitalRightsManagement.Infrastructure.Authentication;

internal sealed class BasicAuthenticationHandler(
    UserManager<AuthUser> userManager,
    SignInManager<AuthUser> signInManager,
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authHeaderValues))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        if (!BasicAuthenticationHeaderValue.TryParse(authHeaderValues, out var authHeader))
        {
            return AuthenticateResult.Fail("Invalid Authorization Scheme");
        }

        var user = await userManager.FindByNameAsync(authHeader.Username);
        if (user is null)
        {
            return AuthenticateResult.Fail("Invalid Username or Password");
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, authHeader.Password, false);
        if (!signInResult.Succeeded)
        {
            return AuthenticateResult.Fail("Invalid Username or Password");
        }

        var roles = await userManager.GetRolesAsync(user);

        Claim[] claims = [
            new(ClaimTypes.Name, user.UserName!, ClaimValueTypes.String, ClaimsIssuer),
            new(ClaimTypes.NameIdentifier, user.DomainUserId.ToString(), ClaimValueTypes.String, ClaimsIssuer),
            new(ClaimTypes.Email, user.Email!, ClaimValueTypes.Email, ClaimsIssuer),
            ..CreateClaimsFromRoles(roles),
        ];

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    private IEnumerable<Claim> CreateClaimsFromRoles(IEnumerable<string> roles) => roles.Select(r => new Claim(ClaimTypes.Role, r, ClaimValueTypes.String, ClaimsIssuer));
}
